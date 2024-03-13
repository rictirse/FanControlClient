using FanControlClient.Model.Setting;
using FanControlClient.ViewModel.Interface;
using Microsoft.Extensions.Options;
using System.Windows;
using System.Windows.Input;

namespace FanControlClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly AppConfigOptions Configuration;
    private readonly IMainWindow MainWindowVM;

#pragma warning disable CS8618
    public MainWindow(
        IMainWindow mainWindowVM,
        IOptions<AppConfigOptions> configuration)
#pragma warning restore CS8618
    { 
        InitializeComponent();

        if (App.ServiceProvider != null)
        {
            MainWindowVM = mainWindowVM;
            DataContext = Configuration = configuration.Value;
        }
    }

    private void move(object sender, MouseButtonEventArgs e) => DragMove();
}