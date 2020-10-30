using System.Windows;

namespace SurveyApp.Service
{
    public class WindowService
    {
        public void ShowWindow<T>(object viewModel) where T : Window, new()
        {
            var window = new T();
            window.DataContext = viewModel;
            window.Show();
        }

        public void ShowDialog<T>(object viewModel) where T : Window, new()
        {
            var dialog = new T();
            dialog.DataContext = viewModel;
            dialog.ShowDialog();
        }
    }
}
