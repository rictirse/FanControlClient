using FanControlClient.Model.Setting;
using FanControlClient.Service.Interface;
using FanControlClient.ViewModel.Interface;
using Microsoft.Extensions.Options;
using System.Collections.Immutable;
using System.Configuration;
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
        var commandLine = (sender as string) ?? string.Empty;

        if (commandLine.Length == 0) return;

        var commands = commandLine.Split(",", StringSplitOptions.RemoveEmptyEntries).ToImmutableArray();

        SetModel(commands);
    }

    private void SetModel(IEnumerable<string> commands)
    {
        foreach (var cmd in commands) 
        {
            float tmp = 0f;
            switch (cmd[0])
            {
                case 'p': //pump RPM
                    tmp = CommandParse(cmd);
                    if (tmp > 0) Configuration.PumpRPM = (int)tmp;
                    break;
                case 'f': //fan RPM
                    tmp = CommandParse(cmd);
                    if (tmp > 0) Configuration.FanRPM = (int)tmp;
                    break;
                case 'r': //room temp
                    tmp = CommandParse(cmd);
                    if (tmp > 0) Configuration.RoomTemp = tmp;
                    break;
                case 'i': //water in
                    tmp = CommandParse(cmd);
                    if (tmp > 0) Configuration.WaterInTemp = tmp;
                    break;
                case 'o': //water out
                    tmp = CommandParse(cmd);
                    if (tmp > 0) Configuration.WaterOutTemp = tmp;
                    break;
                default:
                    break;
            }
        }
    }

    static float CommandParse(string str)
    {
        if (str.Length <= 1) return 0f;
        float temp;
        var tmp = str.Substring(1, str.Length - 1);

        return float.TryParse(tmp, out temp)
            ? temp
            : 0f;
    }
}
