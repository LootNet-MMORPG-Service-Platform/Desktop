using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using desktop_app.Models;

namespace desktop_app.Services;

public static class DialogService
{
    private static IBrush GetBrush(string key, IBrush fallback)
    {
        if (Application.Current?.TryGetResource(key, Application.Current.ActualThemeVariant, out var value) == true
            && value is IBrush brush)
        {
            return brush;
        }

        return fallback;
    }

    private static Window CreateBaseDialog(Control content, double width, double height)
    {
        return new Window
        {
            Width = width,
            Height = height,
            CanResize = false,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new Border
            {
                Background = GetBrush("WhiteBrush", Brushes.White),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20),
                BoxShadow = BoxShadows.Parse("0 10 30 0 #14000000"),
                Child = content
            }
        };
    }

    public static async Task<bool> ShowConfirmDialogAsync(Window owner, string message)
    {
        var tcs = new TaskCompletionSource<bool>();

        var confirmButton = new Button
        {
            Content = "Block",
            Width = 90,
            Height = 36,
            Classes = { "dialogConfirmBtn" }
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
            Children =
            {
                new StackPanel
                {
                    Spacing = 12,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = message,
                            FontSize = 14,
                            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextWrapping = TextWrapping.Wrap,
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Spacing = 12,
                            Margin = new Thickness(0, 6, 0, 0),
                            Children =
                            {
                                confirmButton,
                                cancelButton
                            }
                        }
                    }
                }
            }
        };

        var dialog = CreateBaseDialog(content, 340, 165);
        dialog.Title = "Confirm";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(false);
            dialog.Close();
        };

        confirmButton.Click += (_, _) =>
        {
            tcs.TrySetResult(true);
            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
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

    public static async Task ShowErrorDialogAsync(Window owner, string message)
    {
        var okButton = new Button
        {
            Content = "OK",
            Width = 90,
            Height = 36,
            Classes = { "dialogConfirmBtn" }
        };

        var content = new StackPanel
        {
            Spacing = 12,
            Children =
            {
                new TextBlock
                {
                    Text = "Error",
                    FontSize = 16,
                    FontWeight = FontWeight.SemiBold,
                    Foreground = GetBrush("TextPrimaryBrush", Brushes.Black)
                },
                new SelectableTextBlock
                {
                    Text = message,
                    Foreground = GetBrush("TextPrimaryBrush", Brushes.Black),
                    TextWrapping = TextWrapping.Wrap
                },
                okButton
            }
        };

        var dialog = CreateBaseDialog(content, 500, 300);

        okButton.Click += (_, _) => dialog.Close();

        await dialog.ShowDialog(owner);
    }

    public static async Task ShowInventoryDialogAsync(Window owner, ItemCollectionDTO items)
    {
        var root = new StackPanel { Spacing = 16 };

        root.Children.Add(new TextBlock
        {
            Text = "Inventory",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black)
        });

        root.Children.Add(new TextBlock
        {
            Text = "Weapons",
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black)
        });

        if (items.Weapons.Count == 0)
        {
            root.Children.Add(new TextBlock
            {
                Text = "No weapons",
                Foreground = GetBrush("TextMutedBrush", Brushes.Gray)
            });
        }
        else
        {
            foreach (var w in items.Weapons)
            {
                root.Children.Add(new Border
                {
                    Background = GetBrush("CardBrush", new SolidColorBrush(Color.Parse("#F1F5F9"))),
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(12),
                    Child = new StackPanel
                    {
                        Spacing = 4,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = w.Name,
                                FontWeight = FontWeight.SemiBold,
                                Foreground = GetBrush("TextPrimaryBrush", Brushes.Black)
                            },
                            new TextBlock
                            {
                                Text = $"Type: {w.WeaponType}",
                                Foreground = GetBrush("TextMutedBrush", Brushes.Gray)
                            },
                            new TextBlock
                            {
                                Text = $"Cut: {Math.Round(w.Cut, 2)} | Blunt: {Math.Round(w.Blunt, 2)}",
                                Foreground = GetBrush("TextPrimaryBrush", Brushes.Black)
                            }
                        }
                    }
                });
            }
        }

        root.Children.Add(new TextBlock
        {
            Text = "Armors",
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black),
            Margin = new Thickness(0, 10, 0, 0)
        });

        if (items.Armors.Count == 0)
        {
            root.Children.Add(new TextBlock
            {
                Text = "No armors",
                Foreground = GetBrush("TextMutedBrush", Brushes.Gray)
            });
        }
        else
        {
            foreach (var a in items.Armors)
            {
                root.Children.Add(new Border
                {
                    Background = GetBrush("CardBrush", new SolidColorBrush(Color.Parse("#F1F5F9"))),
                    CornerRadius = new CornerRadius(10),
                    Padding = new Thickness(12),
                    Child = new StackPanel
                    {
                        Spacing = 4,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = a.Name,
                                FontWeight = FontWeight.SemiBold,
                                Foreground = GetBrush("TextPrimaryBrush", Brushes.Black)
                            },
                            new TextBlock
                            {
                                Text = $"Type: {a.ArmorType}",
                                Foreground = GetBrush("TextMutedBrush", Brushes.Gray)
                            },
                            new TextBlock
                            {
                                Text = $"CutRes: {Math.Round(a.CutResistance, 2)} | BluntRes: {Math.Round(a.BluntResistance, 2)}",
                                Foreground = GetBrush("TextPrimaryBrush", Brushes.Black)
                            }
                        }
                    }
                });
            }
        }

        var dialog = CreateBaseDialog(
            new ScrollViewer
            {
                AllowAutoHide = false,
                Margin = new Thickness(0, 0, -16, 0),
                Content = new Border
                {
                    Padding = new Thickness(0, 0, 20, 0),
                    Child = root
                }
            },
            500,
            550
        );

        dialog.Title = "Inventory";

        await dialog.ShowDialog(owner);
    }

    public static async Task ShowEquipmentDialogAsync(Window owner, EquipmentResponseDTO eq)
    {
        var root = new StackPanel { Spacing = 16 };

        root.Children.Add(new TextBlock
        {
            Text = "Equipment",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black)
        });

        root.Children.Add(new TextBlock
        {
            Text = "Armor",
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black)
        });

        void AddSlot(string label, string? value)
        {
            root.Children.Add(new Border
            {
                Background = GetBrush("CardBrush", new SolidColorBrush(Color.Parse("#F1F5F9"))),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(12),
                Child = new StackPanel
                {
                    Spacing = 2,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = label,
                            Foreground = GetBrush("TextMutedBrush", Brushes.Gray)
                        },
                        new TextBlock
                        {
                            Text = value ?? "Empty",
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black)
                        }
                    }
                }
            });
        }

        AddSlot("Head", eq.Head?.Name);
        AddSlot("Body", eq.Body?.Name);
        AddSlot("Gloves", eq.Gloves?.Name);
        AddSlot("Legs", eq.Legs?.Name);
        AddSlot("Boots", eq.Boots?.Name);

        root.Children.Add(new TextBlock
        {
            Text = "Weapons",
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black),
            Margin = new Thickness(0, 10, 0, 0)
        });

        AddSlot("Slot 1", eq.Weapon1?.Name);
        AddSlot("Slot 2", eq.Weapon2?.Name);
        AddSlot("Slot 3", eq.Weapon3?.Name);
        AddSlot("Slot 4", eq.Weapon4?.Name);

        var dialog = CreateBaseDialog(
            new ScrollViewer
            {
                AllowAutoHide = false,
                Margin = new Thickness(0, 0, -16, 0),
                Content = new Border
                {
                    Padding = new Thickness(0, 0, 20, 0),
                    Child = root
                }
            },
            400,
            500
        );

        dialog.Title = "Equipment";

        await dialog.ShowDialog(owner);
    }
}