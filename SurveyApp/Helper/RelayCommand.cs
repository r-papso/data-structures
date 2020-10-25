using System;
using System.Windows.Input;

namespace SurveyApp.Helper
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _predicate;

        public RelayCommand(Action<object> execute) : this(execute, null)
        { }

        public RelayCommand(Action<object> execute, Predicate<object> predicate)
        {
            _execute = execute;
            _predicate = predicate;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _predicate == null ? true : _predicate(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
