using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loop.Confluence.IoC
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InstancePerRequestAttribute : IOCAttribute
    {
        public InstancePerRequestAttribute() { }
        public InstancePerRequestAttribute(Type targetType) : base(targetType) { }

        public override void Register<ClassType>(IServiceLocator container)
        {
            container.RegisterInstancePerRequest<ClassType>(this.TargetType);
        }

        public override void Register(Type classType, IServiceLocator container)
        {
            container.RegisterInstancePerRequest(classType, this.TargetType);
        }
    }
}
