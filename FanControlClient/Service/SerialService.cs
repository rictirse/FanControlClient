using FanControlClient.Model.Setting;
using FanControlClient.Service.Interface;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO.Ports;

namespace FanControlClient.Service;

public class SerialService : ISerialService
{
    private readonly AppConfigOptions Configuration;
    private readonly SerialPort SerialPort = new SerialPort();
    public bool Status { get; private set; }
    public event EventHandler DataUpdate = null!;
    private string bufferStr = string.Empty; 

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
        Status = true;
        SerialPort.PortName = "COM6";

        // 檢查 PORT 是否關閉
        if (!SerialPort.IsOpen)
            SerialPort.Close();

        SerialPort.BaudRate = 9600;
        SerialPort.Parity = Parity.None;
        SerialPort.StopBits = StopBits.One;
        SerialPort.DataBits = 8;
        SerialPort.DataReceived += async (s, e) => await DataReceivedAsync(s, e);
        //SerialPort.DataReceived += DataReceived;

        try
        {
            SerialPort.Open();
        }
        catch (InvalidOperationException e)
        {
            Status = false;
            Debug.WriteLine(e);
        }

        if (Status)
        {
            SerialPort.DiscardInBuffer();  // RX
            SerialPort.DiscardOutBuffer(); // TX
        }

        return Status;
    }

    private void DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        const int bufferSize = 256;
        char[] buffer = new char[bufferSize];
        SerialPort.Read(buffer, 0, bufferSize);
        var str = $"{bufferStr}{string.Join("", buffer).Replace("\0", null)}";

        var pos = GetCommaPos(str);
        var send = str.Substring(0, pos);
        bufferStr = str.Substring(pos, str.Length - pos);

        if (DataUpdate is null
            || pos == -1
            || string.IsNullOrEmpty(send)) return;
        //Debug.WriteLine(send);

        DataUpdate(send, e);
    }

    private async Task DataReceivedAsync(object sender, SerialDataReceivedEventArgs e)
    {
        const int bufferSize = 256;
        var receiveBuffer = new byte[bufferSize];
        var numBytesRead = await SerialPort.BaseStream.ReadAsync(receiveBuffer, 0, bufferSize);
        var str = SerialPort.Encoding.GetString(receiveBuffer, 0, numBytesRead);
        str = $"{bufferStr}{str}";

        var pos = GetCommaPos(str);
        var send = str.Substring(0, pos);
        bufferStr = str.Substring(pos, str.Length - pos);

        if (DataUpdate is null
            || pos == -1
            || string.IsNullOrEmpty(send)) return;
        //Debug.WriteLine(send);

        DataUpdate(send, e);
    }

    private int GetCommaPos(string str)
    {
        int offset = 0;
        int pos = 0;
        while (true)
        {
            pos = str.IndexOf(",", offset + 1);
            if (pos == -1) break;
            offset = Math.Max(pos, offset);
        }

        return offset;
    }

    public async Task SendToSerialAsync(string cmd)
    {
        var byteStr = SerialPort.Encoding.GetBytes($"{cmd}\n");
        await SerialPort.BaseStream.WriteAsync(byteStr, 0, byteStr.Length);
        await SerialPort.BaseStream.FlushAsync();
    }

    public void SendToSerial(string cmd)
    {
        int bufferSize = cmd.Length + 1;
        var buffer = $"{cmd}\n".ToCharArray(0, bufferSize);

        for (int i = 0; i < bufferSize; i++)
        {
            SerialPort.Write(buffer, i, 1);
        }
    }
}