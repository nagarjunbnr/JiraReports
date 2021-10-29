using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.View
{
    public abstract class ViewAttributeBase : Attribute
    {
        public Type ViewModelType { get; private set; }

        public ViewAttributeBase(Type viewModelType)
        {
            this.ViewModelType = viewModelType;
        }
    }
}
