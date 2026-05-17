using System;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using desktop_app.Models;

namespace desktop_app.Services;

public static class UserDialogService
{
    private static IBrush GetBrush(string key, IBrush fallback)
    {
        return DialogWindowFactory.GetBrush(key, fallback);
    }

    private static Window CreateBaseDialog(Control content, double width, double height)
    {
        return DialogWindowFactory.CreateBaseDialog(content, width, height);
    }

    public static async Task<string?> ShowChangeRoleDialogAsync(Window owner, string username, string currentRole)
    {
        var tcs = new TaskCompletionSource<string?>();

        var roles = new[]
        {
            "Player",
            "GameModerator",
            "Admin"
        };

        var comboBox = new ComboBox
        {
            ItemsSource = roles,
            SelectedItem = currentRole,
            Width = 220,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var saveButton = new Button
        {
            Content = "Save",
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
                            Text = $"Change role for {username}",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },
                        comboBox,
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Spacing = 12,
                            Margin = new Thickness(0, 6, 0, 0),
                            Children =
                            {
                                saveButton,
                                cancelButton
                            }
                        }
                    }
                }
            }
        };

        var dialog = CreateBaseDialog(content, 360, 205);
        dialog.Title = "Change role";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        saveButton.Click += (_, _) =>
        {
            tcs.TrySetResult(comboBox.SelectedItem as string);
            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }

    public static async Task ShowInventoryDialogAsync(Window owner, ItemCollectionDTO items)
    {
        var root = new StackPanel { Spacing = 16 };

        root.Children.Add(CreateTitle("Inventory"));
        root.Children.Add(CreateSectionHeader("Weapons"));

        if (items.Weapons.Count == 0)
        {
            root.Children.Add(CreateEmptyText("No weapons"));
        }
        else
        {
            foreach (var weapon in items.Weapons)
            {
                root.Children.Add(CreateWeaponCard(weapon));
            }
        }

        root.Children.Add(CreateSectionHeader("Armors", new Thickness(0, 10, 0, 0)));

        if (items.Armors.Count == 0)
        {
            root.Children.Add(CreateEmptyText("No armors"));
        }
        else
        {
            foreach (var armor in items.Armors)
            {
                root.Children.Add(CreateArmorCard(armor));
            }
        }

        var dialog = CreateBaseDialog(CreateScrollableContent(root), 500, 550);
        dialog.Title = "Inventory";

        await dialog.ShowDialog(owner);
    }

    public static async Task ShowEquipmentDialogAsync(Window owner, EquipmentResponseDTO eq)
    {
        var root = new StackPanel { Spacing = 16 };

        root.Children.Add(CreateTitle("Equipment"));
        root.Children.Add(CreateSectionHeader("Armor"));

        AddEquipmentSlot(root, "Head", eq.Head?.Name);
        AddEquipmentSlot(root, "Body", eq.Body?.Name);
        AddEquipmentSlot(root, "Gloves", eq.Gloves?.Name);
        AddEquipmentSlot(root, "Legs", eq.Legs?.Name);
        AddEquipmentSlot(root, "Boots", eq.Boots?.Name);

        root.Children.Add(CreateSectionHeader("Weapons", new Thickness(0, 10, 0, 0)));

        AddEquipmentSlot(root, "Slot 1", eq.Weapon1?.Name);
        AddEquipmentSlot(root, "Slot 2", eq.Weapon2?.Name);
        AddEquipmentSlot(root, "Slot 3", eq.Weapon3?.Name);
        AddEquipmentSlot(root, "Slot 4", eq.Weapon4?.Name);

        var dialog = CreateBaseDialog(CreateScrollableContent(root), 400, 500);
        dialog.Title = "Equipment";

        await dialog.ShowDialog(owner);
    }

    private static Control CreateScrollableContent(Control content)
    {
        return new ScrollViewer
        {
            AllowAutoHide = false,
            Margin = new Thickness(0, 0, -16, 0),
            Content = new Border
            {
                Padding = new Thickness(0, 0, 20, 0),
                Child = content
            }
        };
    }

    private static TextBlock CreateTitle(string text)
    {
        return new TextBlock
        {
            Text = text,
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black)
        };
    }

    private static TextBlock CreateSectionHeader(string text, Thickness? margin = null)
    {
        return new TextBlock
        {
            Text = text,
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black),
            Margin = margin ?? new Thickness(0)
        };
    }

    private static TextBlock CreateEmptyText(string text)
    {
        return new TextBlock
        {
            Text = text,
            Foreground = GetBrush("TextMutedBrush", Brushes.Gray)
        };
    }

    private static Border CreateWeaponCard(WeaponDTO weapon)
    {
        return CreateItemCard(new StackPanel
        {
            Spacing = 4,
            Children =
            {
                CreatePrimaryText(weapon.Name, FontWeight.SemiBold),
                CreatePrimaryText(
                    $"Cut: {Math.Round(weapon.Cut, 2).ToString(CultureInfo.CurrentCulture)} | Blunt: {Math.Round(weapon.Blunt, 2).ToString(CultureInfo.CurrentCulture)}")
            }
        });
    }

    private static Border CreateArmorCard(ArmorDTO armor)
    {
        return CreateItemCard(new StackPanel
        {
            Spacing = 4,
            Children =
            {
                CreatePrimaryText(armor.Name, FontWeight.SemiBold),
                CreatePrimaryText(
                    $"CutRes: {Math.Round(armor.CutResistance, 2).ToString(CultureInfo.CurrentCulture)} | BluntRes: {Math.Round(armor.BluntResistance, 2).ToString(CultureInfo.CurrentCulture)}")
            }
        });
    }

    private static void AddEquipmentSlot(StackPanel root, string label, string? value)
    {
        root.Children.Add(CreateItemCard(new StackPanel
        {
            Spacing = 2,
            Children =
            {
                CreateMutedText(label),
                CreatePrimaryText(value ?? "Empty", FontWeight.SemiBold)
            }
        }));
    }

    private static Border CreateItemCard(Control content)
    {
        return new Border
        {
            Background = GetBrush("CardBrush", new SolidColorBrush(Color.Parse("#F1F5F9"))),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(12),
            Child = content
        };
    }

    private static TextBlock CreatePrimaryText(string text, FontWeight? fontWeight = null)
    {
        return new TextBlock
        {
            Text = text,
            FontWeight = fontWeight ?? FontWeight.Normal,
            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black)
        };
    }

    private static TextBlock CreateMutedText(string text)
    {
        return new TextBlock
        {
            Text = text,
            Foreground = GetBrush("TextMutedBrush", Brushes.Gray)
        };
    }
}