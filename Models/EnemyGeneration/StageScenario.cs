using System;

namespace desktop_app.Models.EnemyGeneration;

public class StageScenario
{
    public Guid Id { get; set; }
    public Guid StageProfileId { get; set; }
    public int EnemyCount { get; set; }
    public double Weight { get; set; }
}
