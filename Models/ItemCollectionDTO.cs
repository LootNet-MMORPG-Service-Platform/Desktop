using System.Collections.Generic;

namespace desktop_app.Models;

public class ItemCollectionDTO
{
    public List<WeaponDTO> Weapons { get; set; } = [];
    public List<ArmorDTO> Armors { get; set; } = [];
}