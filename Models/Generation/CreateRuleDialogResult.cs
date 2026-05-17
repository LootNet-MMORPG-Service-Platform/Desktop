using desktop_app.Enums;

namespace desktop_app.Models.Generation;

public class CreateRuleDialogResult
{
    public ItemCategory Category { get; set; }
    public WeaponType? WeaponType { get; set; }
    public ArmorType? ArmorType { get; set; }
    public bool IsFallback { get; set; }
}