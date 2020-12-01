using SurveyApp.Helper;
using SurveyApp.Service;
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace SurveyApp.ViewModel
{
    public class DatabaseViewModel : ViewModelBase
    {
        private string _folderPath;
        private readonly LocationManager _locationManager;

        public bool CreateNew { get; set; }

        public int ClusterSize { get; set; }

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

        public ICommand BrowserCommand { get; private set; }

        public DatabaseViewModel() : base()
        { }

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
