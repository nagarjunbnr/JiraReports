using Autofac;
using Autofac.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Loop.Confluence.IoC
{
    public class ServiceLocator : IServiceLocator
    {
        private static object _coreMutex = new object();
        private static bool _isInitialized = false;

        public static ServiceLocator Instance { get; private set; }

        private ContainerBuilder Builder { get; set; }
        private IContainer Container { get; set; }

        static ServiceLocator()
        {
            Instance = new ServiceLocator();
            Instance.GatherComponents();
        }

        private ServiceLocator()
        {
            this.Builder = new ContainerBuilder();
        }

        private void BuildContainer()
        {
            lock (_coreMutex)
            {
                if (this.Container == null)
                {
                    this.Container = this.Builder.Build();
                }
            }
        }

        public object GetObject(Type objectType)
        {
            BuildContainer();

            return this.Container.Resolve(objectType);
        }

        public ObjectType GetObject<ObjectType>()
        {
            BuildContainer();

            return this.Container.Resolve<ObjectType>();
        }

        public ObjectType GetObject<ObjectType, ParameterType>(ParameterType parameter)
        {
            BuildContainer();
            return this.Container.Resolve<ObjectType>(new TypedParameter(typeof(ParameterType), parameter));
        }

        public ObjectType GetObject<ObjectType, Parameter1Type, Parameter2Type>(Parameter1Type parameter1, Parameter2Type parameter2)
        {
            BuildContainer();
            return this.Container.Resolve<ObjectType>(
                new TypedParameter(typeof(Parameter1Type), parameter1),
                new TypedParameter(typeof(Parameter2Type), parameter2));
        }

        public ObjectType GetObject<ObjectType, Parameter1Type, Parameter2Type, Parameter3Type>(Parameter1Type parameter1, Parameter2Type parameter2, Parameter3Type parameter3)
        {
            BuildContainer();
            return this.Container.Resolve<ObjectType>(
                new TypedParameter(typeof(Parameter1Type), parameter1),
                new TypedParameter(typeof(Parameter2Type), parameter2),
                new TypedParameter(typeof(Parameter3Type), parameter3));
        }

        public void RegisterInstance<ClassType>(ClassType classInstance, Type asType = null)
            where ClassType : class
        {
            var regBuilder = this.Builder.RegisterInstance(classInstance);
            RegisterAs(regBuilder, asType);
        }

        public void RegisterInstancePerRequest<ClassType>(Type asType = null)
        {
            var regBuilder = this.Builder.RegisterType<ClassType>();
            RegisterAs(regBuilder, asType);
            regBuilder.InstancePerDependency();
        }

        public void RegisterInstancePerRequest(Type classType, Type asType = null)
        {
            var regBuilder = this.Builder.RegisterType(classType);
            RegisterAs(regBuilder, asType);
            regBuilder.InstancePerDependency();
        }

        public void RegisterSingleInstance<ClassType>(Type asType = null)
        {
            var regBuilder = this.Builder.RegisterType<ClassType>();
            RegisterAs(regBuilder, asType);
            regBuilder.SingleInstance();
        }

        public void RegisterSingleInstance(Type classType, Type asType = null)
        {
            var regBuilder = this.Builder.RegisterType(classType);
            RegisterAs(regBuilder, asType);
            regBuilder.SingleInstance();
        }

        public void RegisterSingleInstancePerScope<ClassType>(Type asType = null)
        {
            var regBuilder = this.Builder.RegisterType<ClassType>();
            RegisterAs(regBuilder, asType);
            regBuilder.InstancePerLifetimeScope();
        }

        public void RegisterSingleInstancePerScope(Type classType, Type asType = null)
        {
            var regBuilder = this.Builder.RegisterType(classType);
            RegisterAs(regBuilder, asType);
            regBuilder.InstancePerLifetimeScope();
        }


        private void RegisterAs<ClassType>(IRegistrationBuilder<ClassType, IConcreteActivatorData, SingleRegistrationStyle> regBuilder,
            Type asType = null)
        {
            if (asType == null)
                return;

            regBuilder.As(asType);
        }


        private void GatherComponents()
        {
            lock (_coreMutex)
            {
                if (_isInitialized) return;

                this.RegisterInstance(this, typeof(IServiceLocator));
                RegisterDecoratedTypes(Assembly.GetExecutingAssembly());

                _isInitialized = true;
            }
        }

        public void RegisterDecoratedTypes(Assembly assembly)
        {
            IEnumerable<(Type ClassType, List<IOCAttribute> Attributes)> iocRules = GetContainerClasses(assembly);
            foreach ((Type classType, List<IOCAttribute> attrs) in iocRules)
            {
                foreach (IOCAttribute attribute in attrs)
                {
                    attribute.Register(classType, this);
                }
            }
        }

        private IEnumerable<(Type ClassType, List<IOCAttribute> Attributes)> GetContainerClasses(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                // Get all attributes, if any. If not, default to InstacePerRequest
                List<IOCAttribute> attrs = type.GetCustomAttributes<IOCAttribute>(true).ToList();
                if (!attrs.Any())
                {
                    continue;
                }

                yield return (type, attrs.ToList());
            }
        }

    }
}
