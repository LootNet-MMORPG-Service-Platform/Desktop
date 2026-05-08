using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using desktop_app.Enums;
using desktop_app.Models.Generation;
using System.Collections.Generic;

namespace desktop_app.Services;

public static class GenerationDialogService
{
    private static IBrush GetBrush(string key, IBrush fallback)
    {
        return DialogWindowFactory.GetBrush(key, fallback);
    }

    private static Window CreateBaseDialog(Control content, double width, double height)
    {
        return DialogWindowFactory.CreateBaseDialog(content, width, height);
    }

    public static async Task<string?> ShowCreateProfileDialogAsync(Window owner)
    {
        var tcs = new TaskCompletionSource<string?>();

        var textBox = new TextBox
        {
            Width = 220,
            Watermark = "Profile name"
        };

        var createButton = new Button
        {
            Content = "Create",
            Width = 90,
            Height = 36,
            Classes = { "detailsBtn" }
        };

        var cancelButton = new Button
        {
            Content = "Cancel",
            Width = 90,
            Height = 36,
            Classes = { "dialogCancelBtn" }
        };

        var content = new Grid
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Children =
            {
                new StackPanel
                {
                    Spacing = 12,
                    Width = 240,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Create generation profile",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        textBox,

                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Spacing = 12,
                            Margin = new Thickness(0, 6, 0, 0),
                            Children =
                            {
                                createButton,
                                cancelButton
                            }
                        }
                    }
                }
            }
        };

        var dialog = CreateBaseDialog(content, 360, 205);
        dialog.Title = "Create profile";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            tcs.TrySetResult(textBox.Text);
            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }
    
    public static async Task<CreateRuleDialogResult?> ShowCreateRuleDialogAsync(Window owner)
    {
        var tcs = new TaskCompletionSource<CreateRuleDialogResult?>();

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

        var fallbackBox = new CheckBox
        {
            Content = "Fallback rule",
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var createButton = new Button
        {
            Content = "Create",
            Width = 90,
            Height = 36,
            Classes = { "detailsBtn" }
        };

        var cancelButton = new Button
        {
            Content = "Cancel",
            Width = 90,
            Height = 36,
            Classes = { "dialogCancelBtn" }
        };

        categoryBox.SelectionChanged += (_, _) =>
        {
            var selectedCategory = (ItemCategory?)categoryBox.SelectedItem;

            weaponTypeBox.IsVisible = selectedCategory == ItemCategory.Weapon;
            armorTypeBox.IsVisible = selectedCategory == ItemCategory.Armor;
        };

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
                            Text = "Create generation rule",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        new TextBlock
                        {
                            Text = "Category",
                            Foreground = GetBrush("TextSecondaryBrush", Brushes.Gray)
                        },
                        categoryBox,

                        new TextBlock
                        {
                            Text = "Weapon type",
                            Foreground = GetBrush("TextSecondaryBrush", Brushes.Gray)
                        },
                        weaponTypeBox,

                        new TextBlock
                        {
                            Text = "Armor type",
                            Foreground = GetBrush("TextSecondaryBrush", Brushes.Gray),
                            IsVisible = false
                        },
                        armorTypeBox,

                        fallbackBox,

                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Spacing = 12,
                            Margin = new Thickness(0, 6, 0, 0),
                            Children =
                            {
                                createButton,
                                cancelButton
                            }
                        }
                    }
                }
            }
        };

        var stack = (StackPanel)(content).Children[0];
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

        var dialog = CreateBaseDialog(content, 380, 360);
        dialog.Title = "Create rule";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            var category = (ItemCategory)categoryBox.SelectedItem!;

            tcs.TrySetResult(new CreateRuleDialogResult
            {
                Category = category,
                WeaponType = category == ItemCategory.Weapon
                    ? (WeaponType?)weaponTypeBox.SelectedItem
                    : null,
                ArmorType = category == ItemCategory.Armor
                    ? (ArmorType?)armorTypeBox.SelectedItem
                    : null,
                IsFallback = fallbackBox.IsChecked == true
            });

            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }

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

        var createButton = new Button
        {
            Content = "Create",
            Width = 90,
            Height = 36,
            Classes = { "detailsBtn" }
        };

        var cancelButton = new Button
        {
            Content = "Cancel",
            Width = 90,
            Height = 36,
            Classes = { "dialogCancelBtn" }
        };

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
                            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        new TextBlock
                        {
                            Text = "Category",
                            Foreground = GetBrush("TextSecondaryBrush", Brushes.Gray)
                        },
                        categoryBox,

                        new TextBlock
                        {
                            Text = "Weapon type",
                            Foreground = GetBrush("TextSecondaryBrush", Brushes.Gray)
                        },
                        weaponTypeBox,

                        new TextBlock
                        {
                            Text = "Armor type",
                            Foreground = GetBrush("TextSecondaryBrush", Brushes.Gray),
                            IsVisible = false
                        },
                        armorTypeBox,

                        weightBox,
                        errorText,

                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Spacing = 12,
                            Margin = new Thickness(0, 6, 0, 0),
                            Children =
                            {
                                createButton,
                                cancelButton
                            }
                        }
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

        var dialog = CreateBaseDialog(content, 380, 395);
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
    
    public static async Task<CreateParameterDialogResult?> ShowCreateParameterDialogAsync(Window owner)
    {
        var tcs = new TaskCompletionSource<CreateParameterDialogResult?>();

        var parameters = Enum.GetValues<ItemParameter>();

        var parameterBox = new ComboBox
        {
            ItemsSource = parameters,
            SelectedItem = ItemParameter.CutDamage,
            Width = 220,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var minBox = new TextBox
        {
            Watermark = "Min",
            Width = 220
        };

        var maxBox = new TextBox
        {
            Watermark = "Max",
            Width = 220
        };

        var weightBox = new TextBox
        {
            Watermark = "Weight",
            Width = 220
        };

        var createButton = new Button
        {
            Content = "Create",
            Width = 90,
            Height = 36,
            Classes = { "detailsBtn" }
        };

        var cancelButton = new Button
        {
            Content = "Cancel",
            Width = 90,
            Height = 36,
            Classes = { "dialogCancelBtn" }
        };

        var content = new Grid
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Children =
            {
                new StackPanel
                {
                    Spacing = 12,
                    Width = 240,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Create parameter",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        parameterBox,
                        minBox,
                        maxBox,
                        weightBox,

                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Spacing = 12,
                            Margin = new Thickness(0, 6, 0, 0),
                            Children =
                            {
                                createButton,
                                cancelButton
                            }
                        }
                    }
                }
            }
        };

        var dialog = CreateBaseDialog(content, 360, 340);
        dialog.Title = "Create parameter";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            if (!double.TryParse(minBox.Text, out var min))
                return;

            if (!double.TryParse(maxBox.Text, out var max))
                return;

            if (!int.TryParse(weightBox.Text, out var weight))
                return;

            tcs.TrySetResult(new CreateParameterDialogResult
            {
                Parameter = (ItemParameter)parameterBox.SelectedItem!,
                Segments = new List<CreateSegmentInput>
                {
                    new()
                    {
                        Min = min,
                        Max = max,
                        Weight = weight
                    }
                }
            });

            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }
    
    public static async Task<CreateElementDialogResult?> ShowCreateElementDialogAsync(Window owner)
    {
        var tcs = new TaskCompletionSource<CreateElementDialogResult?>();

        var elements = Enum.GetValues<ItemElementType>();

        var elementBox = new ComboBox
        {
            ItemsSource = elements,
            SelectedItem = ItemElementType.Fire,
            Width = 220,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var minBox = new TextBox
        {
            Watermark = "Min",
            Width = 220
        };

        var maxBox = new TextBox
        {
            Watermark = "Max",
            Width = 220
        };

        var weightBox = new TextBox
        {
            Watermark = "Weight",
            Width = 220
        };

        var createButton = new Button
        {
            Content = "Create",
            Width = 90,
            Height = 36,
            Classes = { "detailsBtn" }
        };

        var cancelButton = new Button
        {
            Content = "Cancel",
            Width = 90,
            Height = 36,
            Classes = { "dialogCancelBtn" }
        };

        var content = new Grid
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Children =
            {
                new StackPanel
                {
                    Spacing = 12,
                    Width = 240,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Create element",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        elementBox,
                        minBox,
                        maxBox,
                        weightBox,

                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Spacing = 12,
                            Margin = new Thickness(0, 6, 0, 0),
                            Children =
                            {
                                createButton,
                                cancelButton
                            }
                        }
                    }
                }
            }
        };

        var dialog = CreateBaseDialog(content, 360, 340);
        dialog.Title = "Create element";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            if (!double.TryParse(minBox.Text, out var min))
                return;

            if (!double.TryParse(maxBox.Text, out var max))
                return;

            if (!int.TryParse(weightBox.Text, out var weight))
                return;

            tcs.TrySetResult(new CreateElementDialogResult
            {
                ElementType = (ItemElementType)elementBox.SelectedItem!,
                Segments = new List<CreateSegmentInput>
                {
                    new()
                    {
                        Min = min,
                        Max = max,
                        Weight = weight
                    }
                }
            });

            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }
}
