using SurveyApp.Helper;
using SurveyApp.Model;
using SurveyApp.Service;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SurveyApp.ViewModel
{
    /// <summary>
    /// View model used by <see cref="View.LocationWindow"/>
    /// </summary>
    public class LocationViewModel : INotifyPropertyChanged
    {
        private Location _updatingLocation;
        private Location _newLocation;
        private readonly LocationManager _locationManager;

        /// <summary>
        /// Represents new location to be created
        /// </summary>
        public Location NewLocation
        {
            get => _newLocation;
            private set
            {
                _newLocation = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Provides binding <see cref="Submit(object)"/> method execution
        /// </summary>
        public ICommand SubmitCommand { get; private set; }

        /// <summary>
        /// Event invoked when <see cref="NewLocation"/> property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public LocationViewModel() { }

        /// <summary>
        /// Constructor used by <see cref="Microsoft.Extensions.DependencyInjection"/>
        /// </summary>
        /// <param name="locationManager">Instance of <see cref="LocationManager"/></param>
        public LocationViewModel(LocationManager locationManager)
        {
            _locationManager = locationManager;

            InitRelayCommands();
        }

        /// <summary>
        /// Prepares its internal state for updating <paramref name="locationToUpdate"/>
        /// </summary>
        /// <param name="locationToUpdate">Location to be updated</param>
        public void Updating(Location locationToUpdate)
        {
            _updatingLocation = locationToUpdate;
            NewLocation = new Location(_updatingLocation);
        }

        /// <summary>
        /// Prepares its interal state for creating new location
        /// </summary>
        public void New()
        {
            _updatingLocation = null;
            NewLocation = new Location();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Submit(object parameter)
        {
            if (_updatingLocation != null)
            {
                _locationManager.UpdateLocation(_updatingLocation, NewLocation);
                Updating(NewLocation);
            }
            else
            {
                _locationManager.InsertLocation(NewLocation);
                New();
            }
        }

        private void InitRelayCommands()
        {
            SubmitCommand = new MeasurableRelayCommand(Submit);
        }
    }
}
