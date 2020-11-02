using SurveyApp.Helper;
using SurveyApp.Model;
using SurveyApp.Service;
using SurveyApp.View;
using System.Windows.Forms;
using System.Windows.Input;

namespace SurveyApp.ViewModel
{
    public class MainViewModel
    {
        private readonly LocationManager _locationManager;
        private readonly WindowService _windowService;
        private readonly GenerateViewModel _generateViewModel;
        private readonly LocationViewModel _locationViewModel;

        public SearchCriteria SearchCriteria { get; } = new SearchCriteria();

        public Location SelectedLocation { get; set; }

        public CollectionAdapter<Location> Properties { get; }

        public CollectionAdapter<Location> Sites { get; }

        public Helper.Timer Timer => Helper.Timer.Instance;

        public ICommand SearchCommand { get; private set; }

        public ICommand ResetCommand { get; private set; }

        public ICommand NewCommand { get; private set; }

        public ICommand UpdateCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand GenerateCommand { get; private set; }

        public ICommand LoadCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public MainViewModel() { }

        public MainViewModel(LocationManager locationManager, WindowService windowService, GenerateViewModel generateViewModel, LocationViewModel locationViewModel)
        {
            _locationManager = locationManager;
            _windowService = windowService;
            _generateViewModel = generateViewModel;
            _locationViewModel = locationViewModel;

            Properties = _locationManager.Properties;
            Sites = _locationManager.Sites;

            InitRelayCommands();
        }

        private void Search(object parameter) => _locationManager.FindLocationsByCriteria(SearchCriteria);

        private void Reset(object parameter)
        {
            SearchCriteria.MinLatitude = 0;
            SearchCriteria.MaxLatitude = 0;
            SearchCriteria.MinLongitude = 0;
            SearchCriteria.MaxLongitude = 0;

            _locationManager.Reset();
        }

        private void New(object parameter)
        {
            _locationViewModel.New();
            _windowService.ShowDialog<LocationWindow>(_locationViewModel);
        }

        private bool CanUpdate(object parameter) => SelectedLocation != null;

        private void Update(object parameter)
        {
            _locationViewModel.Updating(SelectedLocation);
            _windowService.ShowDialog<LocationWindow>(_locationViewModel);
        }

        private bool CanDelete(object parameter) => SelectedLocation != null;

        private void Delete(object parameter) => _locationManager.DeleteLocation(SelectedLocation);

        private void Generate(object parameter) => _windowService.ShowDialog<GenerateWindow>(_generateViewModel);

        private void Load(object parameter)
        {
            var folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                _locationManager.LoadLocations(folderDialog.SelectedPath);
            }
        }

        private void Save(object parameter)
        {
            var folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                _locationManager.SaveLocations(folderDialog.SelectedPath);
            }
        }

        private void InitRelayCommands()
        {
            SearchCommand = new MeasurableRelayCommand(Search);
            ResetCommand = new MeasurableRelayCommand(Reset);
            NewCommand = new RelayCommand(New);
            UpdateCommand = new RelayCommand(Update, CanUpdate);
            DeleteCommand = new MeasurableRelayCommand(Delete, CanDelete);
            GenerateCommand = new RelayCommand(Generate);
            LoadCommand = new MeasurableRelayCommand(Load);
            SaveCommand = new MeasurableRelayCommand(Save);
        }
    }
}
