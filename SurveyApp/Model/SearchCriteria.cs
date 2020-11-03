using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SurveyApp.Model
{
    /// <summary>
    /// Model object used in <see cref="View.MainWindow"/>
    /// </summary>
    public class SearchCriteria : INotifyPropertyChanged
    {
        private LocationType _locationType;
        private double _minLatitude;
        private double _maxLatitude;
        private double _minLongitude;
        private double _maxLongitude;

        /// <summary>
        /// <see cref="Model.LocationType"/> of searched locations
        /// </summary>
        public LocationType LocationType
        {
            get => _locationType;
            set
            {
                _locationType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Minimal <see cref="Location.Latitude"/> of searched locations
        /// </summary>
        public double MinLatitude
        {
            get => _minLatitude;
            set
            {
                _minLatitude = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Maximal <see cref="Location.Latitude"/> of searched locations
        /// </summary>
        public double MaxLatitude
        {
            get => _maxLatitude;
            set
            {
                _maxLatitude = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Minimal <see cref="Location.Longitude"/> of searched locations
        /// </summary>
        public double MinLongitude
        {
            get => _minLongitude;
            set
            {
                _minLongitude = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Maximal <see cref="Location.Longitude"/> of searched locations
        /// </summary>
        public double MaxLongitude
        {
            get => _maxLongitude;
            set
            {
                _maxLongitude = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Event invoked when one of the properties changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
