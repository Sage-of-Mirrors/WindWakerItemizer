using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WindWakerItemizer
{
    internal class RelayCommand : ICommand
    {
        #region Fields
        private readonly Action<object?>? _execute;
        private readonly Predicate<object?>? _canExecute;
        #endregion

        #region Properties
        public event EventHandler? CanExecuteChanged;
        #endregion

        public RelayCommand(Action<object?>? execute) : this(execute, null)
        {

        }

        public RelayCommand(Action<object?>? execute, Predicate<object?>? canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            if (_execute != null)
            {
                _execute(parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
