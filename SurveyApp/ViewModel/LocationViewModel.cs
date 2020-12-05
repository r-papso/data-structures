using SurveyApp.Helper;
using SurveyApp.Model;
using SurveyApp.Service;
using System;
using System.Windows;
using System.Windows.Input;

namespace SurveyApp.ViewModel
{
    /// <summary>
    /// View model used by <see cref="View.LocationWindow"/>
    /// </summary>
    public class LocationViewModel : ViewModelBase
    {
        private readonly LocationManager _locationManager;

        /// <summary>
        /// Represents location to be created / updated
        /// </summary>
        public Location Location { get; private set; } = new Location();

        /// <summary>
        /// Provides binding <see cref="Add(object)"/> method execution
        /// </summary>
        public ICommand AddCommand { get; private set; }

        /// <summary>
        /// Provides binding <see cref="Update(object)"/> method execution
        /// </summary>
        public ICommand UpdateCommand { get; private set; }

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

        private void Add(object parameter)
        {
            try
            {
                _locationManager.InsertLocation(Location);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }

        private void Update(object parameter)
        {
            try
            {
                _locationManager.UpdateLocation(Location, Location);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }

        private void InitRelayCommands()
        {
            AddCommand = new RelayCommand(StartMeasurement, Add, StopMeasurement);
            UpdateCommand = new RelayCommand(StartMeasurement, Update, StopMeasurement);
        }
    }
}
