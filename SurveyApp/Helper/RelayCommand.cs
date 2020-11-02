using System;
using System.Windows.Input;

namespace SurveyApp.Helper
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Predicate<object> _predicate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">Action invoked by this command</param>
        public RelayCommand(Action<object> action) : this(action, null)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="action">Action invoked by this command</param>
        /// <param name="predicate">Predicate determining if <paramref name="action"/> can be invoked</param>
        public RelayCommand(Action<object> action, Predicate<object> predicate)
        {
            _action = action;
            _predicate = predicate;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null</param>
        /// <returns>True if this command can be executed, otherwise false</returns>
        public virtual bool CanExecute(object parameter)
        {
            return _predicate == null ? true : _predicate(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null</param>
        public virtual void Execute(object parameter)
        {
            _action(parameter);
        }
    }
}
