using System;
using System.Collections.Generic;

namespace desktop_app.Models;

public class ArmorDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int Category { get; set; }
    public int ArmorType { get; set; }
    public double CutResistance { get; set; }
    public double BluntResistance { get; set; }
    public List<ItemElementDTO> Elements { get; set; } = [];
}