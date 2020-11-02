using System;

namespace SurveyApp.Helper
{
    public class MeasurableRelayCommand : RelayCommand
    {
        public MeasurableRelayCommand(Action<object> action) : this(action, null)
        { }

        public MeasurableRelayCommand(Action<object> action, Predicate<object> predicate) : base(action, predicate)
        { }

        public override void Execute(object parameter)
        {
            Timer.Instance.Start();
            base.Execute(parameter);
            Timer.Instance.Stop();
        }
    }
}
