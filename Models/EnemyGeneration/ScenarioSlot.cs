using System;

namespace desktop_app.Models.EnemyGeneration;

public class ScenarioSlot
{
    public Guid Id { get; set; }
    public Guid ScenarioId { get; set; }
    public int Position { get; set; }
    public Guid ClassProfileId { get; set; }
    public string ClassProfileName { get; set; } = "";
    public double Weight { get; set; }

    public string ClassProfileDisplay => string.IsNullOrWhiteSpace(ClassProfileName)
        ? ClassProfileId.ToString()
        : ClassProfileName;
}
