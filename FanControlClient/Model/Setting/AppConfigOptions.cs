using FanControlClient.ViewModel.Base;
using System.Text.Json.Serialization;

namespace FanControlClient.Model.Setting;

public class AppConfigOptions : PropertyBase
{
    public event EventHandler SetPumpEvnet = null!;
    public event EventHandler SetFanEvent = null!;

    public int PumpPwm
    {
        get => _PumpPwm;
        set 
        {
            SetProperty(ref _PumpPwm, value);
            if (SetPumpEvnet is null) return;
            SetPumpEvnet(_PumpPwm, new EventArgs());
        }
    }
    private int _PumpPwm = 30;

    public int FanPwm
    {
        get => _FanPwm;
        set
        { 
            SetProperty(ref _FanPwm, value);
            if (SetFanEvent is null) return;
            SetFanEvent(_FanPwm, new EventArgs());
        }

    }
    private int _FanPwm = 0;

    [JsonIgnore]
    public int FanRPM
    {
        get => _FanRPM;
        set => SetProperty(ref _FanRPM, value);

    }
    private int _FanRPM = 0;

    [JsonIgnore]
    public int PumpRPM
    {
        get => _PumpRPM;
        set => SetProperty(ref _PumpRPM, value);

    }
    private int _PumpRPM = 0;

    [JsonIgnore]
    public float WaterInTemp
    {
        get => _WaterInTemp;
        set => SetProperty(ref _WaterInTemp, value);

    }
    private float _WaterInTemp = 0f;

    [JsonIgnore]
    public float WaterOutTemp
    {
        get => _WaterOutTemp;
        set => SetProperty(ref _WaterOutTemp, value);

    }
    private float _WaterOutTemp = 0f;

    [JsonIgnore]
    public float RoomTemp
    {
        get => _RoomTemp;
        set => SetProperty(ref _RoomTemp, value);

    }
    private float _RoomTemp = 0f;
}