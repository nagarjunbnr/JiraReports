using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JiraReports.ViewModel
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public static ContentControl ViewPort { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void OnInitialize()
        {

        }

        public virtual void OnLoad()
        {

        }

        public virtual bool Validate()
        {
            return true;
        }
    }
}
