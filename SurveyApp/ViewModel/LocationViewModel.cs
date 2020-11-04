using SurveyApp.Helper;
using SurveyApp.Model;
using SurveyApp.Service;
using System.Windows.Input;

namespace SurveyApp.ViewModel
{
    /// <summary>
    /// View model used by <see cref="View.LocationWindow"/>
    /// </summary>
    public class LocationViewModel : ViewModelBase
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
        /// Default constructor
        /// </summary>
        public LocationViewModel() : base()
        { }

        /// <summary>
        /// Constructor used by <see cref="Microsoft.Extensions.DependencyInjection"/>
        /// </summary>
        /// <param name="locationManager">Instance of <see cref="LocationManager"/></param>
        public LocationViewModel(LocationManager locationManager) : base()
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
            SubmitCommand = new RelayCommand(StartMeasurement, Submit, StopMeasurement);
        }
    }
}
