using Structures.Interface;
using SurveyApp.Adapter;
using SurveyApp.Helper;
using SurveyApp.Interface;
using SurveyApp.Service;
using SurveyApp.View;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SurveyApp.ViewModel
{
    /// <summary>
    /// View model used by <see cref="MainWindow"/>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly GenerateViewModel _generateViewModel;
        private readonly LocationViewModel _locationViewModel;
        private readonly DatabaseViewModel _databaseViewModel;

        /// <summary>
        /// ID of location to be searched / deleted
        /// </summary>
        public int SelectedId { get; set; }

        /// <summary>
        /// Collection of registered locations
        /// </summary>
        public HashFileAdapter<ISerializable> Locations => Manager.Serializables;

        /// <summary>
        /// <see cref="Helper.Timer"/> instance
        /// </summary>
        public Timer Timer => Timer.Instance;

        /// <summary>
        /// Provides binding for <see cref="Search(object)"/> method execution
        /// </summary>
        public ICommand SearchCommand { get; private set; }

        /// <summary>
        /// Provides binding for <see cref="Manage(object)"/> method execution
        /// </summary>
        public ICommand ManageCommand { get; private set; }

        /// <summary>
        /// Provides binding for <see cref="Delete(object)"/> method execution
        /// </summary>
        public ICommand DeleteCommand { get; private set; }

        /// <summary>
        /// Provides binding for <see cref="Generate(object)"/> method execution
        /// </summary>
        public ICommand GenerateCommand { get; private set; }

        /// <summary>
        /// Provides binding for <see cref="NewDatabase(object)"/> method execution
        /// </summary>
        public ICommand NewDatabaseCommand { get; private set; }

        /// <summary>
        /// Provides binding for <see cref="LoadDatabase(object)"/> method execution
        /// </summary>
        public ICommand LoadDatabaseCommand { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainViewModel() : base()
        { }

        /// <summary>
        /// Constructor used by <see cref="Microsoft.Extensions.DependencyInjection"/>
        /// </summary>
        /// <param name="manager"><see cref="IManager"/> instance</param>
        /// <param name="generateViewModel"><see cref="GenerateViewModel"/> instance</param>
        /// <param name="locationViewModel"><see cref="LocationViewModel"/> instance</param>
        /// <param name="databaseViewModel"><see cref="DatabaseViewModel"/> instance</param>
        public MainViewModel(IManager manager, GenerateViewModel generateViewModel, LocationViewModel locationViewModel,
                             DatabaseViewModel databaseViewModel) : base(manager)
        {
            _generateViewModel = generateViewModel;
            _locationViewModel = locationViewModel;
            _databaseViewModel = databaseViewModel;

            InitRelayCommands();
        }

        /// <summary>
        /// Called on application closing
        /// </summary>
        /// <param name="sender">Object that raised the event</param>
        /// <param name="e">Information about the event</param>
        public void OnClosing(object sender, CancelEventArgs e) => Manager.Release();

        private bool CanSearch(object parameter) => Locations.PrimaryFile != null;

        private void Search(object parameter)
        {
            try
            {
                Manager.Find(SelectedId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }

        private bool CanManage(object parameter) => Locations.PrimaryFile != null;

        private void Manage(object parameter) => WindowService.ShowDialog<LocationWindow>(_locationViewModel);

        private bool CanDelete(object parameter) => Locations.PrimaryFile != null;

        private void Delete(object parameter)
        {
            try
            {
                Manager.Delete(SelectedId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }

        private bool CanGenerate(object parameter) => Locations.PrimaryFile != null;

        private void Generate(object parameter) => WindowService.ShowDialog<GenerateWindow>(_generateViewModel);

        private void NewDatabase(object parameter)
        {
            _databaseViewModel.CreateNew = true;
            WindowService.ShowDialog<DatabaseWindow>(_databaseViewModel);
        }

        private void LoadDatabase(object paramter)
        {
            _databaseViewModel.CreateNew = false;
            WindowService.ShowDialog<DatabaseWindow>(_databaseViewModel);
        }

        private void InitRelayCommands()
        {
            SearchCommand = new RelayCommand(CanSearch, StartMeasurement, Search, StopMeasurement);
            ManageCommand = new RelayCommand(CanManage, Manage);
            DeleteCommand = new RelayCommand(CanDelete, StartMeasurement, Delete, StopMeasurement);
            GenerateCommand = new RelayCommand(CanGenerate, Generate);
            NewDatabaseCommand = new RelayCommand(NewDatabase);
            LoadDatabaseCommand = new RelayCommand(LoadDatabase);
        }
    }
}
