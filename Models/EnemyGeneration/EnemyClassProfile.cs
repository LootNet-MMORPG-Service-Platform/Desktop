using System;
using System.Collections.Generic;
using desktop_app.Enums;

namespace desktop_app.Models.EnemyGeneration;

public class EnemyClassProfile
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public EnemyClass Class { get; set; }
    public List<int> AllowedColumns { get; set; } = new();
    public Guid GenerationProfileId { get; set; }
    public double Weight { get; set; }
}
