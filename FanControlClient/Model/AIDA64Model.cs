using System.Diagnostics.CodeAnalysis;

namespace FanControlClient.Model;

public class AIDA64Model
{
    public required string Id { get; init; }
    public required string Label { get; init; }
    public string? Value { get; private set; }

    [SetsRequiredMembers]
    public AIDA64Model(string id, string label)
    {
        Id = id;
        Label = label;
    }

    public void SetValue(string value) => Value = value;

    public override string ToString() => $"{Id},{Label}:[{Value}]";
}