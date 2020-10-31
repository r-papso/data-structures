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
        private IEnumerable<Location> _properties;
        private IEnumerable<Location> _sites;

        private readonly LocationManager _locationManager;
        private readonly WindowService _windowService;
        private readonly GenerateViewModel _generateViewModel;
        private readonly LocationViewModel _locationViewModel;

        public SearchCriteria SearchCriteria { get; } = new SearchCriteria();

        public Location SelectedLocation { get; set; }

        public CollectionAdapter<Location> Properties { get; }

        public CollectionAdapter<Location> Sites { get; }

        /*public IEnumerable<Location> Properties
        {
            get => _properties;
            private set
            {
                _properties = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<Location> Sites
        {
            get => _sites;
            private set
            {
                _sites = value;
                OnPropertyChanged();
            }
        }*/

        //public ObservableCollection<Location> Properties { get; set; }

        //public ObservableCollection<Location> Sites { get; set; }

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

        public MainViewModel(LocationManager locationManager, WindowService windowService, GenerateViewModel generateViewModel, LocationViewModel locationViewModel)
        {
            _locationManager = locationManager;
            _windowService = windowService;
            _generateViewModel = generateViewModel;
            _locationViewModel = locationViewModel;

            Properties = _locationManager.Properties;
            Sites = _locationManager.Sites;

            InitRelayCommands();
            //RegisterListeners();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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

        }

        private void Save(object parameter)
        {

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

        /*private void RegisterListeners()
        {
            _locationManager.LocationsChanged += LocationsChanged;
        }*/

        /*private void LocationsChanged(object sender, Event.LocationsChangedEventArgs args)
        {
            if (args.Properties != null)
            {
                Properties = new LinkedList<Location>(args.Properties);
            }
            if (args.Sites != null)
            {
                Sites = new LinkedList<Location>(args.Sites);
            }
        }*/
    }
}
