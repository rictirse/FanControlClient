using System.Text.Json.Serialization;

namespace FanControlClient.Model;

public class SerialModel
{
    [JsonPropertyName("pumpRPM")]
    public int PumpRPM { get; set; }
    [JsonPropertyName("fanRPM")]
    public int FanRPM { get; set; }
    [JsonPropertyName("room")]
    public float RoomTemp { get; set; }
    [JsonPropertyName("waterIn")]
    public float WaterInTemp { get; set; }
    [JsonPropertyName("waterOut")]
    public float WaterOutTemp { get; set; }
}