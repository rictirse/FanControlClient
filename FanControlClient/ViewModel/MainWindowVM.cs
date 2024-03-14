using FanControlClient.Model;
using FanControlClient.Model.Setting;
using FanControlClient.Service.Interface;
using FanControlClient.ViewModel.Interface;
using Microsoft.Extensions.Options;
using System.Collections.Immutable;
using System.Configuration;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Threading;

namespace FanControlClient.ViewModel;

public class MainWindowVM : IMainWindow
{
    private readonly AppConfigOptions Configuration;
    private readonly ISerialService SerialSvc;
    private readonly IAIDA64ReaderService AIDA64ReaderSvc;
    private readonly DispatcherTimer UpdateTimer;

    public MainWindowVM(
        ISerialService serialSvc,
        IAIDA64ReaderService aida64ReaderSvc,
        IOptions<AppConfigOptions> configuration)
    {
        SerialSvc = serialSvc;
        AIDA64ReaderSvc = aida64ReaderSvc;
        Configuration = configuration.Value;
        SerialSvc.DataUpdate += SerialDataUpdate;
        UpdateTimer = new DispatcherTimer();
        UpdateTimer.Interval = TimeSpan.FromSeconds(0.5);
        UpdateTimer.Tick += OnTime;
        UpdateTimer.Start();
    }

    private void OnTime(object? sender, EventArgs e)
    {
        AIDA64ReaderSvc.UpdateData();
        float? value = AIDA64ReaderSvc.GetFloat("TCPU");
        if (value != null)
        { 
            Configuration.CPUTemp = (float)value;
        }
        value = AIDA64ReaderSvc.GetFloat("SCPUUTI");
        if (value != null)
        { 
            Configuration.CPUUsage = (float)value;
        }
    }

    private void SerialDataUpdate(object? sender, EventArgs e)
    {
        var commandLine = (string?)sender ?? string.Empty;

        if (string.IsNullOrEmpty(commandLine)) return;

        try
        {
            var serialMode = JsonSerializer.Deserialize<SerialModel>(commandLine);
            if (serialMode == null) return;
            if (serialMode.PumpRPM > 0)
            {
                Configuration.PumpRPM = serialMode.PumpRPM;
            }
            if (serialMode.FanRPM > 0)
            {
                Configuration.FanRPM = serialMode.FanRPM;
            }
            if (serialMode.RoomTemp > 0)
            { 
                Configuration.RoomTemp = serialMode.RoomTemp;
            }
            if (serialMode.WaterInTemp > 0)
            {
                Configuration.WaterInTemp = serialMode.WaterInTemp;
            }
            if (serialMode.WaterOutTemp > 0)
            { 
                Configuration.WaterOutTemp = serialMode.WaterOutTemp;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}
