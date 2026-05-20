using System;

namespace desktop_app.Models.EnemyGeneration;

public class StageProfile
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int StageIndex { get; set; }
    public double Weight { get; set; }
    public double Falloff { get; set; }
    public int Threshold { get; set; }
}
