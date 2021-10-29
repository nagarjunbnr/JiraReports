using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.View
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewContainerAttribute : ViewAttributeBase
    {
        public ViewContainerAttribute(Type viewModelType) : base(viewModelType)
        {
            
        }
    }
}
