using System;
using System.Collections.Generic;
using desktop_app.Enums;

namespace desktop_app.Models.Generation;

public class GenerationRule
{
    public Guid Id { get; set; }
    public ItemCategory Category { get; set; }
    public WeaponType? WeaponType { get; set; }
    public ArmorType? ArmorType { get; set; }
    public bool IsFallback { get; set; }

    public bool HasParameters => Parameters.Count > 0;
    public List<GenerationParameter> Parameters { get; set; } = new();
    
    public bool HasElements => Elements.Count > 0;
    public List<GenerationElement> Elements { get; set; } = new();
}