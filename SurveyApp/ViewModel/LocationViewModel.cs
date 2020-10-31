using SurveyApp.Helper;
using SurveyApp.Model;
using SurveyApp.Service;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SurveyApp.ViewModel
{
    public class LocationViewModel : INotifyPropertyChanged
    {
        private Location _updatingLocation;
        private Location _newLocation;
        private readonly LocationManager _locationManager;

        public Location NewLocation
        {
            get => _newLocation;
            private set
            {
                _newLocation = value;
                OnPropertyChanged();
            }
        }

        public ICommand SubmitCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public LocationViewModel() { }

        public LocationViewModel(LocationManager locationManager)
        {
            _locationManager = locationManager;

            InitRelayCommands();
        }

        public void Updating(Location locationToUpdate)
        {
            _updatingLocation = locationToUpdate;
            NewLocation = new Location(_updatingLocation);
        }

        public void New()
        {
            _updatingLocation = null;
            NewLocation = new Location();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
            SubmitCommand = new RelayCommand(Submit);
        }
    }
}
