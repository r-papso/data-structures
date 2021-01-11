using Structures.Interface;
using SurveyApp.Helper;
using SurveyApp.Interface;
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
        /// <summary>
        /// Represents location to be created / updated
        /// </summary>
        public ISerializable Location { get; private set; }

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
        /// <param name="manager">Instance of <see cref="IManager"/></param>
        /// <param name="factory">Instance of <see cref="IFactory"/></param>
        public LocationViewModel(IManager manager, IFactory factory) : base(manager)
        {
            Location = factory.GetSerializable();

            InitRelayCommands();
        }

        private void Add(object parameter)
        {
            try
            {
                Manager.Insert(Location);
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
                Manager.Update(Location, Location);
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
