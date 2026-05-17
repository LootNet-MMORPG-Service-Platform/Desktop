using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using desktop_app.Enums;
using desktop_app.Models.Generation;

namespace desktop_app.Services.Dialogs.Generation;

public static class TypeWeightGenerationDialogs
{
    public static async Task<CreateTypeWeightDialogResult?> ShowCreateTypeWeightDialogAsync(Window owner)
    {
        var tcs = new TaskCompletionSource<CreateTypeWeightDialogResult?>();

        var categories = Enum.GetValues<ItemCategory>();
        var weaponTypes = Enum.GetValues<WeaponType>();
        var armorTypes = Enum.GetValues<ArmorType>();

        var categoryBox = new ComboBox
        {
            ItemsSource = categories,
            SelectedItem = ItemCategory.Weapon,
            Width = 220,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var weaponTypeBox = new ComboBox
        {
            ItemsSource = weaponTypes,
            SelectedItem = WeaponType.Sword,
            Width = 220,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var armorTypeBox = new ComboBox
        {
            ItemsSource = armorTypes,
            SelectedItem = ArmorType.Body,
            Width = 220,
            HorizontalAlignment = HorizontalAlignment.Center,
            IsVisible = false
        };

        var weightBox = new TextBox
        {
            Watermark = "Weight",
            Width = 220
        };

        var errorText = new TextBlock
        {
            Foreground = Brushes.IndianRed,
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            IsVisible = false
        };

        var createButton = GenerationDialogUiFactory.CreateDialogButton("Create", "detailsBtn");
        var cancelButton = GenerationDialogUiFactory.CreateDialogButton("Cancel", "dialogCancelBtn");

        var content = new Grid
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Children =
            {
                new StackPanel
                {
                    Spacing = 12,
                    Width = 250,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Create type weight",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GenerationDialogUiFactory.GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        new TextBlock
                        {
                            Text = "Category",
                            Foreground = GenerationDialogUiFactory.GetBrush("TextSecondaryBrush", Brushes.Gray)
                        },
                        categoryBox,

                        new TextBlock
                        {
                            Text = "Weapon type",
                            Foreground = GenerationDialogUiFactory.GetBrush("TextSecondaryBrush", Brushes.Gray)
                        },
                        weaponTypeBox,

                        new TextBlock
                        {
                            Text = "Armor type",
                            Foreground = GenerationDialogUiFactory.GetBrush("TextSecondaryBrush", Brushes.Gray),
                            IsVisible = false
                        },
                        armorTypeBox,

                        weightBox,
                        errorText,

                        GenerationDialogUiFactory.CreateButtonRow(createButton, cancelButton)
                    }
                }
            }
        };

        var stack = (StackPanel)content.Children[0];
        var weaponLabel = (TextBlock)stack.Children[3];
        var armorLabel = (TextBlock)stack.Children[5];

        categoryBox.SelectionChanged += (_, _) =>
        {
            var selectedCategory = (ItemCategory?)categoryBox.SelectedItem;

            var isWeapon = selectedCategory == ItemCategory.Weapon;
            var isArmor = selectedCategory == ItemCategory.Armor;

            weaponLabel.IsVisible = isWeapon;
            weaponTypeBox.IsVisible = isWeapon;

            armorLabel.IsVisible = isArmor;
            armorTypeBox.IsVisible = isArmor;
        };

        var dialog = GenerationDialogUiFactory.CreateBaseDialog(content, 380, 395);
        dialog.Title = "Create type weight";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            if (!double.TryParse(weightBox.Text, out var weight) || weight <= 0)
            {
                errorText.Text = "Weight must be greater than 0.";
                errorText.IsVisible = true;
                return;
            }

            var category = (ItemCategory)categoryBox.SelectedItem!;
            var weaponType = category == ItemCategory.Weapon
                ? (WeaponType?)weaponTypeBox.SelectedItem
                : null;
            var armorType = category == ItemCategory.Armor
                ? (ArmorType?)armorTypeBox.SelectedItem
                : null;

            if ((weaponType == null && armorType == null) ||
                (weaponType != null && armorType != null))
            {
                errorText.Text = "Choose exactly one item type.";
                errorText.IsVisible = true;
                return;
            }

            tcs.TrySetResult(new CreateTypeWeightDialogResult
            {
                Category = category,
                WeaponType = weaponType,
                ArmorType = armorType,
                Weight = weight
            });

            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }

    public static async Task<CreateTypeWeightDialogResult?> ShowEditTypeWeightDialogAsync(Window owner, TypeWeight weight)
    {
        var tcs = new TaskCompletionSource<CreateTypeWeightDialogResult?>();

        var categories = Enum.GetValues<ItemCategory>();
        var weaponTypes = Enum.GetValues<WeaponType>();
        var armorTypes = Enum.GetValues<ArmorType>();
        var isInitialWeapon = weight.Category == ItemCategory.Weapon;
        var isInitialArmor = weight.Category == ItemCategory.Armor;

        var categoryBox = new ComboBox
        {
            ItemsSource = categories,
            SelectedItem = weight.Category,
            Width = 220,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var weaponTypeBox = new ComboBox
        {
            ItemsSource = weaponTypes,
            SelectedItem = weight.WeaponType ?? WeaponType.Sword,
            Width = 220,
            HorizontalAlignment = HorizontalAlignment.Center,
            IsVisible = isInitialWeapon
        };

        var armorTypeBox = new ComboBox
        {
            ItemsSource = armorTypes,
            SelectedItem = weight.ArmorType ?? ArmorType.Body,
            Width = 220,
            HorizontalAlignment = HorizontalAlignment.Center,
            IsVisible = isInitialArmor
        };

        var weightBox = new TextBox
        {
            Text = weight.Weight.ToString(),
            Watermark = "Weight",
            Width = 220
        };

        var errorText = new TextBlock
        {
            Foreground = Brushes.IndianRed,
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            IsVisible = false
        };

        var saveButton = GenerationDialogUiFactory.CreateDialogButton("Save", "detailsBtn");
        var cancelButton = GenerationDialogUiFactory.CreateDialogButton("Cancel", "dialogCancelBtn");

        var content = new Grid
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Children =
            {
                new StackPanel
                {
                    Spacing = 12,
                    Width = 250,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Edit type weight",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GenerationDialogUiFactory.GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        new TextBlock
                        {
                            Text = "Category",
                            Foreground = GenerationDialogUiFactory.GetBrush("TextSecondaryBrush", Brushes.Gray)
                        },
                        categoryBox,

                        new TextBlock
                        {
                            Text = "Weapon type",
                            Foreground = GenerationDialogUiFactory.GetBrush("TextSecondaryBrush", Brushes.Gray),
                            IsVisible = isInitialWeapon
                        },
                        weaponTypeBox,

                        new TextBlock
                        {
                            Text = "Armor type",
                            Foreground = GenerationDialogUiFactory.GetBrush("TextSecondaryBrush", Brushes.Gray),
                            IsVisible = isInitialArmor
                        },
                        armorTypeBox,

                        weightBox,
                        errorText,

                        GenerationDialogUiFactory.CreateButtonRow(saveButton, cancelButton)
                    }
                }
            }
        };

        var stack = (StackPanel)content.Children[0];
        var weaponLabel = (TextBlock)stack.Children[3];
        var armorLabel = (TextBlock)stack.Children[5];

        categoryBox.SelectionChanged += (_, _) =>
        {
            var selectedCategory = (ItemCategory?)categoryBox.SelectedItem;

            var isWeapon = selectedCategory == ItemCategory.Weapon;
            var isArmor = selectedCategory == ItemCategory.Armor;

            weaponLabel.IsVisible = isWeapon;
            weaponTypeBox.IsVisible = isWeapon;

            armorLabel.IsVisible = isArmor;
            armorTypeBox.IsVisible = isArmor;
        };

        var dialog = GenerationDialogUiFactory.CreateBaseDialog(content, 380, 395);
        dialog.Title = "Edit type weight";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        saveButton.Click += (_, _) =>
        {
            if (!double.TryParse(weightBox.Text, out var newWeight) || newWeight <= 0)
            {
                errorText.Text = "Weight must be greater than 0.";
                errorText.IsVisible = true;
                return;
            }

            var category = (ItemCategory)categoryBox.SelectedItem!;
            var weaponType = category == ItemCategory.Weapon
                ? (WeaponType?)weaponTypeBox.SelectedItem
                : null;
            var armorType = category == ItemCategory.Armor
                ? (ArmorType?)armorTypeBox.SelectedItem
                : null;

            if ((weaponType == null && armorType == null) ||
                (weaponType != null && armorType != null))
            {
                errorText.Text = "Choose exactly one item type.";
                errorText.IsVisible = true;
                return;
            }

            tcs.TrySetResult(new CreateTypeWeightDialogResult
            {
                Category = category,
                WeaponType = weaponType,
                ArmorType = armorType,
                Weight = newWeight
            });

            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }
}
