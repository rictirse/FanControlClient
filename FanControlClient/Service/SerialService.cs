using FanControlClient.Model.Setting;
using FanControlClient.Service.Interface;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;

namespace FanControlClient.Service;

public class SerialService : ISerialService
{
    const int bufferSize = 1024;
    private readonly AppConfigOptions Configuration;
    private readonly SerialPort SerialPort = new SerialPort();
    private Task RxTask = null!;
    public event EventHandler DataUpdate = null!;
    public bool Status => SerialPort.IsOpen;

    public SerialService(IOptions<AppConfigOptions> configuration)
    {
        SerailPortInit();
        Configuration = configuration.Value;
        Configuration.SetFanEvent += async (s, e) => await CallFanEvent(s, e);
        Configuration.SetPumpEvnet += async (s, e) => await CallPumpEvent(s, e);
    }

    private async Task CallPumpEvent(object? sender, EventArgs e)
       => await SendToSerialAsync($"p{(int)sender!}");

    private async Task CallFanEvent(object? sender, EventArgs e)
        => await SendToSerialAsync($"f{(int)sender!}");

    private bool SerailPortInit()
    {
        SerialPort.PortName = "COM6";

        // 檢查 PORT 是否關閉
        if (!SerialPort.IsOpen)
            SerialPort.Close();

        SerialPort.BaudRate = 115200;
        SerialPort.Parity = Parity.None;
        SerialPort.StopBits = StopBits.One;
        SerialPort.DataBits = 8;

        try
        {
            SerialPort.Open();
            RxTask = Task.Run(() => RxWorkAsync());
        }
        catch (InvalidOperationException e)
        {
            Debug.WriteLine(e);
        }

        if (Status)
        {
            SerialPort.DiscardInBuffer();  // RX
            SerialPort.DiscardOutBuffer(); // TX
        }

        return Status;
    }

    private async Task RxWorkAsync()
    {
        var buffer = new byte[bufferSize];
        var bufferStr = new StringBuilder("", bufferSize / 4);

        while (SerialPort.IsOpen)
        {
            var count = await SerialPort.BaseStream.ReadAsync(buffer, 0, bufferSize);

            for (int i = 0; i < count; i++)
            {
                if ((char)buffer[i] == '\n')
                {
                    if (bufferStr.Length != 0)
                    {
                        DataUpdate(bufferStr.ToString(), new EventArgs());
                    }
                    bufferStr.Clear();
                }
                else
                {
                    bufferStr.Append((char)buffer[i]);
                }
            }
        }
    }

    public async Task SendToSerialAsync(string cmd)
    {
        var byteStr = SerialPort.Encoding.GetBytes($"{cmd}\n");
        await SerialPort.BaseStream.WriteAsync(byteStr, 0, byteStr.Length);
        await SerialPort.BaseStream.FlushAsync();
    }
}