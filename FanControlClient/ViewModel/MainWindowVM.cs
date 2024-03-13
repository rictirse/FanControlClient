using FanControlClient.Model.Setting;
using FanControlClient.Service.Interface;
using FanControlClient.ViewModel.Interface;
using Microsoft.Extensions.Options;
using System.Collections.Immutable;

namespace FanControlClient.ViewModel;

public class MainWindowVM : IMainWindow
{
    private readonly AppConfigOptions Configuration;
    private readonly ISerialService SerialSvc;

    public MainWindowVM(
        ISerialService serialSvc,
        IOptions<AppConfigOptions> configuration)
    {
        SerialSvc = serialSvc;
        Configuration = configuration.Value;
        SerialSvc.DataUpdate += SerialDataUpdate;
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
