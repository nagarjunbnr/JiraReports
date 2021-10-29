using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services
{
    public abstract class IOCAttribute : Attribute
    {
        public Type TargetType { get; private set; }

        public IOCAttribute() { }

        public IOCAttribute(Type targetType)
        {
            this.TargetType = targetType;
        }

        public abstract void Register<ClassType>(IServiceLocator container);
        public abstract void Register(Type classType, IServiceLocator container);
    }
}
