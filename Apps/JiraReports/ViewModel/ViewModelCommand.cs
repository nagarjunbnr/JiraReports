using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JiraReports.ViewModel
{
    public class ViewModelCommand : ICommand
    {
        private readonly Action executeAction;
        private readonly Func<bool> canExecute;

        public ViewModelCommand(Action executeAction, Func<bool> canExecute = null)
        {
            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            this.executeAction();
        }

        public bool CanExecute(object parameter)
        {
            return (this.canExecute == null) ? true : this.canExecute();
        }

        public void OnCanExcuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, null);
            }
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
    }

    public class ViewModelCommand<ParameterType> : ICommand
    {
        private readonly Action<ParameterType> executeAction;
        private readonly Func<ParameterType, bool> canExecute;

        public ViewModelCommand(Action<ParameterType> executeAction, Func<ParameterType, bool> canExecute = null)
        {
            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            this.executeAction((ParameterType)parameter);
        }

        public bool CanExecute(object parameter)
        {
            return (this.canExecute == null) ? true : this.canExecute((ParameterType)parameter);
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public void OnCanExcuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, null);
            }
        }
    }
}
