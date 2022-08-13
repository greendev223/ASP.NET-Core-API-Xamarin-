using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MukaiTablet2.ViewModel
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action execute;

        public DelegateCommand(Action execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            execute(); 
        }
        public void Dummy()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());   //
        }
    }
}
