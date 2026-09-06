using System.Text.Json;
using System.Text.Json.Serialization;

namespace BizDataRouter.Models;

public sealed class PiAttribute
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("value")]
    public JsonElement Value { get; set; }

    [JsonPropertyName("quality")]
    public string? Quality { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}

public sealed class PiElement
{
    [JsonPropertyName("element")]
    public string? Element { get; set; }

    [JsonPropertyName("attributeCount")]
    public int AttributeCount { get; set; }

    [JsonPropertyName("attributes")]
    public List<PiAttribute> Attributes { get; set; } = [];
}

public sealed class PiBatch
{
    [JsonPropertyName("batchTimestamp")]
    public DateTime BatchTimestamp { get; set; }

    [JsonPropertyName("piServer")]
    public string? PiServer { get; set; }

    [JsonPropertyName("afDatabase")]
    public string? AfDatabase { get; set; }

    [JsonPropertyName("template")]
    public string? Template { get; set; }

    [JsonPropertyName("elementCount")]
    public int? ElementCount { get; set; }

    [JsonPropertyName("elements")]
    public List<PiElement> Elements { get; set; } = [];
}