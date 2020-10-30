using SurveyApp.Helper;
using SurveyApp.Model;
using SurveyApp.Service;
using SurveyApp.View;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SurveyApp.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private IEnumerable<Location> _displayedProperties;
        private IEnumerable<Location> _displayedSites;

        private LocationManager _locationManager;
        private WindowService _windowService;
        private GenerateViewModel _generateViewModel;

        public SearchCriteria SearchCriteria { get; } = new SearchCriteria();

        public Location SelectedLocation { get; set; }

        public IEnumerable<Location> DisplayedProperties
        {
            get => _displayedProperties;
            set
            {
                _displayedProperties = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<Location> DisplayedSites
        {
            get => _displayedSites;
            set
            {
                _displayedSites = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand { get; private set; }

        public ICommand ResetCommand { get; private set; }

        public ICommand NewCommand { get; private set; }

        public ICommand UpdateCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand GenerateCommand { get; private set; }

        public ICommand LoadCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel() { }

        public MainViewModel(LocationManager locationManager, WindowService windowService, GenerateViewModel generateViewModel)
        {
            _locationManager = locationManager;
            _windowService = windowService;
            _generateViewModel = generateViewModel;

            InitRelayCommands();
            RegisterListeners();
        }

        public void Search(object parameter) => _locationManager.FindLocationsByCriteria(SearchCriteria);

        public void Reset(object parameter)
        {
            SearchCriteria.MinLatitude = 0;
            SearchCriteria.MaxLatitude = 0;
            SearchCriteria.MinLongitude = 0;
            SearchCriteria.MaxLongitude = 0;

            _locationManager.Reset();
        }

        public void New(object parameter)
        {

        }

        public bool CanUpdate(object parameter) => SelectedLocation != null;

        public void Update(object parameter)
        {

        }

        public bool CanDelete(object parameter) => SelectedLocation != null;

        public void Delete(object parameter) => _locationManager.DeleteLocation(SelectedLocation);

        public void Generate(object parameter) => _windowService.ShowDialog<GenerateWindow>(_generateViewModel);

        public void Load(object parameter)
        {

        }

        public void Save(object parameter)
        {

        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void InitRelayCommands()
        {
            SearchCommand = new RelayCommand(Search);
            ResetCommand = new RelayCommand(Reset);
            NewCommand = new RelayCommand(New);
            UpdateCommand = new RelayCommand(Update, CanUpdate);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            GenerateCommand = new RelayCommand(Generate);
            LoadCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);
        }

        private void RegisterListeners()
        {
            _locationManager.LocationsChanged += LocationsChanged;
        }

        private void LocationsChanged(object sender, Event.LocationsChangedEventArgs args)
        {
            DisplayedProperties = args.Properties;
            DisplayedSites = args.Sites;
        }
    }
}
