using System;

namespace desktop_app.Models.EnemyGeneration;

public class CreateScenarioSlotDialogResult
{
    public int Position { get; set; }
    public Guid ClassProfileId { get; set; }
    public double Weight { get; set; }
}
