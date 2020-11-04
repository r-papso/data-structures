using System;
using System.Windows.Input;

namespace SurveyApp.Helper
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object>[] _actions;
        private readonly Predicate<object> _predicate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actions">Actions invoked by this command</param>
        public RelayCommand(params Action<object>[] actions) : this(null, actions)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actions">Actions invoked by this command</param>
        /// <param name="predicate">Predicate determining if <paramref name="actions"/> can be invoked</param>
        public RelayCommand(Predicate<object> predicate, params Action<object>[] actions)
        {
            _actions = actions;
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
        /// Defines the methods to be called when the command is invoked
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null</param>
        public virtual void Execute(object parameter)
        {
            foreach (var action in _actions)
            {
                action(parameter);
            }
        }
    }
}
