using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SurveyApp.Model
{
    public class SearchCriteria : INotifyPropertyChanged
    {
        private LocationType _locationType;
        private double _minLatitude;
        private double _maxLatitude;
        private double _minLongitude;
        private double _maxLongitude;

        public LocationType LocationType
        {
            get => _locationType;
            set
            {
                _locationType = value;
                OnPropertyChanged();
            }
        }

        public double MinLatitude
        {
            get => _minLatitude;
            set
            {
                _minLatitude = value;
                OnPropertyChanged();
            }
        }

        public double MaxLatitude
        {
            get => _maxLatitude;
            set
            {
                _maxLatitude = value;
                OnPropertyChanged();
            }
        }

        public double MinLongitude
        {
            get => _minLongitude;
            set
            {
                _minLongitude = value;
                OnPropertyChanged();
            }
        }

        public double MaxLongitude
        {
            get => _maxLongitude;
            set
            {
                _maxLongitude = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
