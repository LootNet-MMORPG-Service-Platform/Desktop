using System;
using desktop_app.Enums;

namespace desktop_app.Models.Generation;

public class TypeWeight
{
    public Guid Id { get; set; }
    public ItemCategory Category { get; set; }
    public WeaponType? WeaponType { get; set; }
    public ArmorType? ArmorType { get; set; }
    public double Weight { get; set; }
    
    public string TypeDisplay =>
        WeaponType?.ToString() ?? ArmorType?.ToString() ?? "";
}