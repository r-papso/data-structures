using System;

namespace SurveyApp.Helper
{
    public class MeasurableRelayCommand : RelayCommand
    {
        public MeasurableRelayCommand(Action<object> action) : this(action, null)
        { }

        public MeasurableRelayCommand(Action<object> action, Predicate<object> predicate) : base(action, predicate)
        { }

        /// <summary>
        /// Defines the method to be called when the command is invoked, and measures its execution time
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null</param>
        public override void Execute(object parameter)
        {
            Timer.Instance.Start();
            base.Execute(parameter);
            Timer.Instance.Stop();
        }
    }
}
