using System;
using System.Globalization;
using System.Text.Json;

namespace desktop_app.Models;

public class AdminLog
{
    public Guid Id { get; set; }
    public Guid AdminId { get; set; }
    public string Action { get; set; } = "";
    public string TargetUserId { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string? Data { get; set; }

    public string CreatedAtText => CreatedAt.ToString("g", CultureInfo.CurrentCulture);
    public string DataText => string.IsNullOrWhiteSpace(Data) ? "-" : Data;
    public string DataPreview => CreateDataPreview();
    public string DataDetailsText => CreateDataDetailsText();

    private string CreateDataPreview()
    {
        if (string.IsNullOrWhiteSpace(Data))
            return "-";

        var compact = Data.Replace(Environment.NewLine, " ").Replace("\n", " ").Replace("\r", " ").Trim();

        return compact.Length <= 150
            ? compact
            : $"{compact[..150]}...";
    }

    private string CreateDataDetailsText()
    {
        if (string.IsNullOrWhiteSpace(Data))
            return "No log data available.";

        try
        {
            using var document = JsonDocument.Parse(Data);
            return JsonSerializer.Serialize(
                document.RootElement,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
        }
        catch (JsonException)
        {
            return Data;
        }
    }
}
