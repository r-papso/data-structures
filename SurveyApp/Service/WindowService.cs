using System.Windows;

namespace SurveyApp.Service
{
    /// <summary>
    /// Provides operations for creating views with their coresponding view models
    /// </summary>
    public class WindowService
    {
        /// <summary>
        /// Creates and shows window with its coresponding view model
        /// </summary>
        /// <typeparam name="T">Type of window to be shown</typeparam>
        /// <param name="viewModel">Coresponding view model of window</param>
        public void ShowWindow<T>(object viewModel) where T : Window, new()
        {
            var window = new T();
            window.DataContext = viewModel;
            window.Show();
        }

        /// <summary>
        /// Creates and shows window as dialog with its coresponding view model
        /// </summary>
        /// <typeparam name="T">Type of window to be shown as dialog</typeparam>
        /// <param name="viewModel">Coresponding view model of window</param>
        public void ShowDialog<T>(object viewModel) where T : Window, new()
        {
            var dialog = new T();
            dialog.DataContext = viewModel;
            dialog.ShowDialog();
        }
    }
}
