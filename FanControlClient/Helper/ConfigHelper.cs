using FanControlClient.Model.Setting;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.IO;

namespace FanControlClient.Helper;

public static class ConfigHelper
{
    const string SettingFile = "appsettings.json";

    public static IConfiguration CreateConfig()
    {
        if (!File.Exists(SettingFile)
            || string.IsNullOrEmpty(File.ReadAllText(SettingFile)))
        {
            File.WriteAllText(SettingFile, "{\r\n\r\n}");
        }

        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(SettingFile, true, true)
            .Build();
    }

    public static void SaveConfig(this AppConfigOptions opt)
    {
        try
        {
            opt.FileToJson(SettingFile);
        }
        catch (ConfigurationErrorsException)
        {
            Console.WriteLine("Error writing app settings");
        }
    }
}
