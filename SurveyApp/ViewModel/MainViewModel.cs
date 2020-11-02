using SurveyApp.Helper;
using SurveyApp.Model;
using SurveyApp.Service;
using SurveyApp.View;
using System.Windows.Forms;
using System.Windows.Input;

namespace SurveyApp.ViewModel
{
    /// <summary>
    /// View model used by <see cref="MainWindow"/>
    /// </summary>
    public class MainViewModel
    {
        private readonly LocationManager _locationManager;
        private readonly WindowService _windowService;
        private readonly GenerateViewModel _generateViewModel;
        private readonly LocationViewModel _locationViewModel;

        /// <summary>
        /// Criteria of locations to be searched
        /// </summary>
        public SearchCriteria SearchCriteria { get; } = new SearchCriteria();

        /// <summary>
        /// Selected location
        /// </summary>
        public Location SelectedLocation { get; set; }

        /// <summary>
        /// Collection of registered properties
        /// </summary>
        public CollectionAdapter<Location> Properties { get; }

        /// <summary>
        /// Collection of registered sites
        /// </summary>
        public CollectionAdapter<Location> Sites { get; }

        /// <summary>
        /// <see cref="Helper.Timer"/> instance
        /// </summary>
        public Helper.Timer Timer => Helper.Timer.Instance;

        /// <summary>
        /// Provides binding for <see cref="Search(object)"/> method execution
        /// </summary>
        public ICommand SearchCommand { get; private set; }

        /// <summary>
        /// Provides binding for <see cref="Reset(object)"/> method execution
        /// </summary>
        public ICommand ResetCommand { get; private set; }

        /// <summary>
        /// Provides binding for <see cref="New(object)"/> method execution
        /// </summary>
        public ICommand NewCommand { get; private set; }

        /// <summary>
        /// Provides binding for <see cref="Update(object)"/> method execution
        /// </summary>
        public ICommand UpdateCommand { get; private set; }

        /// <summary>
        /// Provides binding for <see cref="Delete(object)"/> method execution
        /// </summary>
        public ICommand DeleteCommand { get; private set; }

        /// <summary>
        /// Provides binding for <see cref="Generate(object)"/> method execution
        /// </summary>
        public ICommand GenerateCommand { get; private set; }

        /// <summary>
        /// Provides binding for <see cref="Load(object)"/> method execution
        /// </summary>
        public ICommand LoadCommand { get; private set; }

        /// <summary>
        /// Provides binding for <see cref="Save(object)"/> method execution
        /// </summary>
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainViewModel() { }

        /// <summary>
        /// Constructor used by <see cref="Microsoft.Extensions.DependencyInjection"/>
        /// </summary>
        /// <param name="locationManager"><see cref="LocationManager"/> instance</param>
        /// <param name="windowService"><see cref="WindowService"/> instance</param>
        /// <param name="generateViewModel"><see cref="GenerateViewModel"/> instance</param>
        /// <param name="locationViewModel"><see cref="LocationManager"/> instance</param>
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
