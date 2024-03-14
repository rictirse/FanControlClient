using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace FanControlClient.Helper;

static class JsonSerializerExtensions
{
    public static JsonSerializerOptions defaultSettings = new JsonSerializerOptions()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = null,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
}

public static class JsonHelper
{
    public static string ToJson<T>(this T payload) =>
        JsonSerializer.Serialize(payload, JsonSerializerExtensions.defaultSettings);

    public static void FileToJson<T>(this T payload, string savePath) =>
        Save(savePath, payload.ToJson());

    public static void Save(string aFilePath, string aContent) =>
        Save(new FileInfo(aFilePath), aContent);

    public static void Save(FileInfo aFi, string aContent)
    {
        try
        {
            if (!aFi.Directory!.Exists)
            {
                aFi.Directory.Create();
            }

            if (aFi.Exists)
            {
                aFi.Delete();
            }

            aFi.Refresh();
            if (aFi.Exists) throw new Exception("寫檔失敗，檔案已存在或已開啟。");

            File.WriteAllText(aFi.FullName, aContent);
        }
        catch { }
    }
}