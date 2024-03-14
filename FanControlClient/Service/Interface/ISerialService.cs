using System.Security.Cryptography;

namespace FanControlClient.Service.Interface;

public interface ISerialService
{
    Task SendToSerialAsync(string cmd);
    event EventHandler DataUpdate;
}