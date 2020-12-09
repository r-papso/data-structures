using Microsoft.Extensions.DependencyInjection;
using SurveyApp.Interface;
using SurveyApp.Model;
using SurveyApp.Service;
using SurveyApp.View;
using SurveyApp.ViewModel;
using System.Windows;

namespace SurveyApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IManager<Location>, LocationManager>();
            services.AddSingleton<GenerateViewModel>();
            services.AddSingleton<LocationViewModel>();
            services.AddSingleton<DatabaseViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
        }
    }
}
