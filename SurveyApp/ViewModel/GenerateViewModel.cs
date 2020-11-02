using SurveyApp.Helper;
using SurveyApp.Model;
using SurveyApp.Service;
using System.Windows.Input;

namespace SurveyApp.ViewModel
{
    public class GenerateViewModel
    {
        private readonly LocationManager _locationManager;

        public GenerationCriteria GenerationCriteria { get; } = new GenerationCriteria();

        public ICommand SubmitCommand { get; private set; }

        public GenerateViewModel() { }

        public GenerateViewModel(LocationManager locationManager)
        {
            _locationManager = locationManager;

            InitRelayCommands();
        }

        private void Submit(object parameter) => _locationManager.GenerateLocations(GenerationCriteria);

        private void InitRelayCommands()
        {
            SubmitCommand = new MeasurableRelayCommand(Submit);
        }
    }
}
