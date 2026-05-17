using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace desktop_app.Models.Economy;

public class MarketStats
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement> Properties { get; set; } = new();
}
