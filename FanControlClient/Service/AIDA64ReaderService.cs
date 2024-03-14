using FanControlClient.AIDA64;
using FanControlClient.Model;
using FanControlClient.Service.Interface;
using System.Xml;

namespace FanControlClient.Service;

public class AIDA64ReaderService : IAIDA64ReaderService
{
    private readonly SharedMemSaver SharedMemSaver;
    private IDictionary<string, AIDA64Model> MainColles { get; init; }

    public AIDA64ReaderService()
    {
        SharedMemSaver = new SharedMemSaver();
        MainColles = new Dictionary<string, AIDA64Model>();
    }

    public void UpdateData()
    {
        SharedMemSaver.OpenView();
        var raw = SharedMemSaver.GetData();
        Parser(raw);
    }

    private void Parser(string? raw)
    {
        if (string.IsNullOrEmpty(raw)) return;

        var doc = new XmlDocument();
        doc.LoadXml($"<html>{raw}</html>");

        var root = doc.FirstChild;

        if (!root?.HasChildNodes ?? true) return;

        foreach (XmlNode rootNode in root!.ChildNodes)
        {
            var nodeName = rootNode.Name;
            try
            {
                if (rootNode.ChildNodes.Count != 3) continue;

                var id = rootNode.ChildNodes[0]!.InnerText;
                var label = rootNode.ChildNodes[1]!.InnerText;
                var value = rootNode.ChildNodes[2]!.InnerText;

                if (!MainColles.ContainsKey(id))
                {
                    MainColles.Add(id, new AIDA64Model(id, label));
                }

                MainColles[id].SetValue(value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nodeName} Parser err\r\n" +
                                  $"{ex.Message}\r\n" +
                                  $"{ex.StackTrace}");
            }
        }
    }

    public string? GetString(string key)
    { 
        return MainColles.TryGetValue(key, out var value) 
            ? value.Value
            : null;
    }

    public float? GetFloat(string key)
    {
        if (!MainColles.TryGetValue(key, out var sValue)) return null;
        float.TryParse(sValue?.Value ?? string.Empty, out var fValue);

        return fValue;
    }
}
