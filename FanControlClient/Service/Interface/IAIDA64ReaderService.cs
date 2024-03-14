using FanControlClient.Model;

namespace FanControlClient.Service.Interface;

public interface IAIDA64ReaderService
{
    void UpdateData();
    string? GetString(string key);
    float? GetFloat(string key);
}
