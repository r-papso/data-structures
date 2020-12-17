using SurveyApp.Helper;
using SurveyApp.Interface;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SurveyApp.ViewModel
{
    /// <summary>
    /// Represents base class for view models
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected IManager Manager { get; }

        /// <summary>
        /// Event invoked when <see cref="NewLocation"/> property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ViewModelBase() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="manager">Manager</param>
        public ViewModelBase(IManager manager) => Manager = manager;

        protected void StartMeasurement(object parameter) => Timer.Instance.Start();

        protected void StopMeasurement(object parameter) => Timer.Instance.Stop();

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
