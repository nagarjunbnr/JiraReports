using JiraReports.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JiraReports.Services
{
    [SingleInstance(typeof(INavigationService))]
    public class NavigationService : INavigationService
    {
        private static object _coreMutex = new object();
        private static bool _isInitialized = false;

        private static Dictionary<string, (Type viewType, Type viewModelType)> views 
            = new Dictionary<string, (Type viewType, Type viewModelType)>(StringComparer.OrdinalIgnoreCase);
        private static Type contentContainerType;
        private static Type contentContainerViewModelType;

        private ContentControl contentContainer;
        private IServiceLocator serviceLocator;

        public NavigationService(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;

            GatherComponents();

            contentContainer = this.serviceLocator.GetObject(contentContainerType) as ContentControl;
        }

        public void StartContainer()
        {
            ViewModel.ViewModel viewModel = this.serviceLocator.GetObject(contentContainerViewModelType) as ViewModel.ViewModel;

            viewModel.OnInitialize();
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                (contentContainer as MainWindow).Show();
                contentContainer.DataContext = viewModel;
            });
            viewModel.OnLoad();
        }

        public void Navigate(string path)
        {
            if(!views.TryGetValue(path, out (Type viewType, Type viewModelType) types))
                throw new ArgumentException("Path not found!");

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                FrameworkElement view = this.serviceLocator.GetObject(types.viewType) as FrameworkElement;
                ViewModel.ViewModel viewModel = this.serviceLocator.GetObject(types.viewModelType) as ViewModel.ViewModel;


                viewModel.OnInitialize();


                view.DataContext = viewModel;
                contentContainer.Content = view;
                viewModel.OnLoad();
            });
        }


        private void GatherComponents()
        {
            lock (_coreMutex)
            {
                if (_isInitialized) return;

                RegisterDecoratedTypes(Assembly.GetCallingAssembly());

                _isInitialized = true;
            }
        }

        public void RegisterDecoratedTypes(Assembly assembly)
        {
            IEnumerable<(Type ClassType, List<ViewAttributeBase> Attributes)> viewClasses = GetViewClasses(assembly);
            foreach ((Type classType, List<ViewAttributeBase> attrs) in viewClasses)
            {
                foreach (ViewAttributeBase attribute in attrs)
                {
                    if(attribute is ViewAttribute view)
                    {
                        views[view.Path] = (classType, attribute.ViewModelType);
                    }
                    else if (attribute is ViewContainerAttribute container)
                    {
                        contentContainerType = classType;
                        contentContainerViewModelType = attribute.ViewModelType;
                    }
                }
            }
        }

        private IEnumerable<(Type ClassType, List<ViewAttributeBase> Attributes)> GetViewClasses(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                // Get all attributes, if any. If not, default to InstacePerRequest
                List<ViewAttributeBase> attrs = type.GetCustomAttributes<ViewAttributeBase>(true).ToList();
                if (!attrs.Any()) 
                    continue;

                yield return (type, attrs.ToList());
            }
        }
    }
}
