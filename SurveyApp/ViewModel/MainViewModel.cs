using SurveyApp.Helper;
using SurveyApp.Model;
using SurveyApp.Service;
using SurveyApp.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SurveyApp.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private IEnumerable<Location> _displayedLocations;

        private LocationManager _locationManager;
        private WindowService _windowService;
        private GenerateViewModel _generateViewModel;

        public SearchCriteria SearchCriteria { get; } = new SearchCriteria();

        public IEnumerable<LocationType> LocationTypeValues => Enum.GetValues(typeof(LocationType)).Cast<LocationType>();

        public Location SelectedLocation { get; set; }

        public IEnumerable<Location> DisplayedLocations
        {
            get => _displayedLocations;
            set
            {
                if (_displayedLocations != value)
                {
                    _displayedLocations = value;
                    OnPropertyChanged();
                }
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

            var locations = new List<Location>();

            var prop1 = new Location()
            {
                ID = 0,
                Description = "Prop 1",
                Latitude = 10,
                Longitude = 20,
                LocationType = LocationType.Property,
                Locations = new LinkedList<Location>()
            };
            var prop2 = new Location()
            {
                ID = 0,
                Description = "Prop 2",
                Latitude = 10,
                Longitude = 20,
                LocationType = LocationType.Property,
                Locations = new LinkedList<Location>()
            };

            var site1 = new Location()
            {
                ID = 0,
                Description = "Site 1",
                Latitude = 10,
                Longitude = 20,
                LocationType = LocationType.Site
            };
            var site2 = new Location()
            {
                ID = 1,
                Description = "Site 2",
                Latitude = 10,
                Longitude = 20,
                LocationType = LocationType.Site
            };

            prop1.Locations.Add(site1);
            prop1.Locations.Add(site2);
            prop2.Locations.Add(site1);
            prop2.Locations.Add(site2);

            locations.Add(prop1);
            locations.Add(prop2);

            DisplayedLocations = locations;
        }

        public bool CanSearch(object parameter) => SearchCriteria.LocationType != null && SearchCriteria.MinLatitude != null && SearchCriteria.MinLongitude != null;

        public void Search(object parameter) => DisplayedLocations = _locationManager.GetLocationsByCriteria(SearchCriteria);

        public void Reset(object parameter)
        {
            SearchCriteria.MinLatitude = null;
            SearchCriteria.MaxLatitude = null;
            SearchCriteria.MinLongitude = null;
            SearchCriteria.MaxLongitude = null;

            DisplayedLocations = SearchCriteria.LocationType == LocationType.Property ? _locationManager.Properties : _locationManager.Sites;
        }

        public void New(object parameter)
        {

        }

        public bool CanUpdate(object parameter) => SelectedLocation != null;

        public void Update(object parameter)
        {

        }

        public bool CanDelete(object parameter) => SelectedLocation != null;

        public void Delete(object parameter)
        {

        }

        public void Generate(object parameter) => _windowService.ShowWindow<GenerateWindow>(_generateViewModel);

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
            SearchCommand = new RelayCommand(Search, CanSearch);
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
            _locationManager.LocationsChanged += (sender, args) => DisplayedLocations = args.Locations;
        }
    }
}
