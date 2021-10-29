using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SingleInstancePerScopeAttribute : IOCAttribute
    {
        public SingleInstancePerScopeAttribute() { }
        public SingleInstancePerScopeAttribute(Type targetType) : base(targetType) { }

        public override void Register<ClassType>(IServiceLocator container)
        {
            container.RegisterSingleInstancePerScope<ClassType>(this.TargetType);
        }

        public override void Register(Type classType, IServiceLocator container)
        {
            container.RegisterSingleInstancePerScope(classType, this.TargetType);
        }
    }
}
