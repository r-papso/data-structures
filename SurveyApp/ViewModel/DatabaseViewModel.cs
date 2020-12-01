using SurveyApp.Helper;
using SurveyApp.Service;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace SurveyApp.ViewModel
{
    /// <summary>
    /// View model used by <see cref="View.DatabaseWindow"/>
    /// </summary>
    public class DatabaseViewModel : ViewModelBase
    {
        private string _folderPath;
        private readonly LocationManager _locationManager;

        /// <summary>
        /// Determines if new database should be created
        /// </summary>
        public bool CreateNew { get; set; }

        /// <summary>
        /// File system's cluster size in bytes
        /// </summary>
        public int ClusterSize { get; set; }

        /// <summary>
        /// Path to directory where database will be created / loaded from
        /// </summary>
        public string FolderPath
        {
            get => _folderPath;
            set
            {
                _folderPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Provides binding for <see cref="Submit(object)"/> method execution
        /// </summary>
        public ICommand SubmitCommand { get; private set; }

        /// <summary>
        /// Provides binding for <see cref="Browse(object)"/> method execution
        /// </summary>
        public ICommand BrowserCommand { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DatabaseViewModel() : base()
        { }

        /// <summary>
        /// Constructor used by <see cref="Microsoft.Extensions.DependencyInjection"/>
        /// </summary>
        /// <param name="locationManager">Instance of <see cref="LocationManager"/></param>
        public DatabaseViewModel(LocationManager locationManager) : base()
        {
            _locationManager = locationManager;

            InitRelayCommands();
        }

        private void Submit(object parameter)
        {
            try
            {
                if (CreateNew)
                {
                    _locationManager.NewDatabase(FolderPath, ClusterSize);
                }
                else
                {
                    _locationManager.LoadDatabase(FolderPath);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }

        private void Browse(object parameter)
        {
            var folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                FolderPath = folderDialog.SelectedPath;
            }
        }

        private void InitRelayCommands()
        {
            SubmitCommand = new RelayCommand(StartMeasurement, Submit, StopMeasurement);
            BrowserCommand = new RelayCommand(Browse);
        }
    }
}
