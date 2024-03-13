using FanControlClient.Helper;
using FanControlClient.Model.Setting;
using FanControlClient.Service;
using FanControlClient.Service.Interface;
using FanControlClient.ViewModel;
using FanControlClient.ViewModel.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FanControlClient;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;
    public static IConfiguration Configuration { get; private set; } = null!;

    public App()
    {
        Configuration = ConfigHelper.CreateConfig();
        var serviceCollection = new ServiceCollection()
            .AddOptions()
            .Configure<AppConfigOptions>(Configuration);

        ConfigureServices(serviceCollection);

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
        => services.AddScoped<MainWindow>()
           .AddScoped<ISerialService, SerialService>()
           .AddScoped<IMainWindow, MainWindowVM>();

    private void AppOnStartup(object sender, StartupEventArgs e)
    {
        var mainWindow = ServiceProvider.GetService<MainWindow>();
        mainWindow?.Show();
    }
}
