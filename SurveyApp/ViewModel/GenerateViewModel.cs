using SurveyApp.Helper;
using SurveyApp.Model;
using SurveyApp.Service;
using System.Windows.Input;

namespace SurveyApp.ViewModel
{
    /// <summary>
    /// View model used by <see cref="View.GenerateWindow"/>
    /// </summary>
    public class GenerateViewModel : ViewModelBase
    {
        private readonly LocationManager _locationManager;

        /// <summary>
        /// Criteria of locations to be generated
        /// </summary>
        public GenerationCriteria GenerationCriteria { get; } = new GenerationCriteria();

        /// <summary>
        /// Provides binding for <see cref="Submit(object)"/> method execution
        /// </summary>
        public ICommand SubmitCommand { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GenerateViewModel() : base()
        { }

        /// <summary>
        /// Constructor used by <see cref="Microsoft.Extensions.DependencyInjection"/>
        /// </summary>
        /// <param name="locationManager">Instance of <see cref="LocationManager"/></param>
        public GenerateViewModel(LocationManager locationManager) : base()
        {
            _locationManager = locationManager;

            InitRelayCommands();
        }

        private void Submit(object parameter) => _locationManager.GenerateLocations(GenerationCriteria);

        private void InitRelayCommands()
        {
            SubmitCommand = new RelayCommand(StartMeasurement, Submit, StopMeasurement);
        }
    }
}
