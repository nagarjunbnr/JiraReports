using JiraReports.Common;
using JiraReports.Services;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace JiraReports.ViewModel.Reports
{
    [InstancePerRequest]
    public abstract class MultiGridViewModel : ReportDisplayModel
    {
        private IServiceLocator serviceLocator;

        public ObservableCollection<MultiGridItemViewModel> Items { get; set; } = new ObservableCollection<MultiGridItemViewModel>();

        public MultiGridViewModel(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        public void ClearModels()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                this.Items.Clear();
            });
        }

        public void SetNextModel(string name, IEnumerable modelData)
        {
            MultiGridItemViewModel itemViewModel = serviceLocator.GetObject<MultiGridItemViewModel>();
            itemViewModel.Name = name;
            itemViewModel.SetModel(modelData);
            
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                this.Items.Add(itemViewModel);
            });
        }
    }
}
