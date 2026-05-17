using System;
using System.Collections.Generic;

namespace desktop_app.Models;

public class WeaponDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int Category { get; set; }
    public int WeaponType { get; set; }
    public double Cut { get; set; }
    public double Blunt { get; set; }
    public List<ItemElementDTO> Elements { get; set; } = [];
}