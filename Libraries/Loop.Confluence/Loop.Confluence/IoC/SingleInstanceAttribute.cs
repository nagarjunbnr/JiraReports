using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loop.Confluence.IoC
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SingleInstanceAttribute : IOCAttribute
    {
        public SingleInstanceAttribute() { }
        public SingleInstanceAttribute(Type targetType) : base(targetType) { }

        public override void Register<ClassType>(IServiceLocator container)
        {
            container.RegisterSingleInstance<ClassType>(this.TargetType);
        }

        public override void Register(Type classType, IServiceLocator container)
        {
            container.RegisterSingleInstance(classType, this.TargetType);
        }
    }
}
