using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using desktop_app.Models;
using desktop_app.ViewModels;

namespace desktop_app.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }

    private async void ToggleBlockStatus_Click(object? sender, RoutedEventArgs _)
    {
        if (sender is Button btn && btn.DataContext is AdminUser user)
        {
            if (DataContext is HomeViewModel vm)
            {
                if (!user.IsBlocked)
                {
                    var result = await ShowConfirmDialog("Are you sure you want to block this user?");
                    if (!result)
                        return;
                }

                await vm.UsersVm.ToggleBlockStatusCommand.ExecuteAsync(user);
            }
        }
    }
    
    private async Task<bool> ShowConfirmDialog(string message)
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

        var owner = VisualRoot as Window;

        var dialog = new Window
        {
            Width = 340,
            Height = 180,
            CanResize = false,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20),
                BoxShadow = BoxShadows.Parse("0 10 30 0 #14000000"),
                Child = new StackPanel
                {
                    Spacing = 12,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = message,
                            FontSize = 14,
                            Foreground = Brushes.Black,
                            TextWrapping = TextWrapping.Wrap
                        },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Spacing = 12,
                            Margin = new Thickness(0, 10, 0, 0),
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

        await dialog.ShowDialog(owner!);
        return await tcs.Task;
    }
    
    private void SelectUser_Click(object? sender, RoutedEventArgs _)
    {
        if (sender is Button btn && btn.DataContext is AdminUser user)
        {
            if (DataContext is HomeViewModel vm)
            {
                vm.UsersVm.SelectUserCommand.Execute(user);
            }
        }
    }
    
    private async Task<string?> ShowChangeRoleDialog(string username, string currentRole)
    {
        var tcs = new TaskCompletionSource<string?>();

        var roles = new[]
        {
            "Player",
            "GameModerator",
            "Admin",
            "SuperAdmin"
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

        var owner = VisualRoot as Window;

        var dialog = new Window
        {
            Width = 360,
            Height = 220,
            CanResize = false,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20),
                BoxShadow = BoxShadows.Parse("0 10 30 0 #14000000"),
                Child = new Grid
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,

                    Children =
                    {
                        new StackPanel
                        {
                            Spacing = 14,
                            Width = 240,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Children =
                            {
                                new TextBlock
                                {
                                    Text = $"Change role for {username}",
                                    FontSize = 16,
                                    FontWeight = FontWeight.SemiBold,
                                    Foreground = Brushes.Black,
                                    TextAlignment = TextAlignment.Center
                                },
                                comboBox,
                                new StackPanel
                                {
                                    Orientation = Orientation.Horizontal,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    Spacing = 12,
                                    Margin = new Thickness(0, 10, 0, 0),
                                    Children =
                                    {
                                        saveButton,
                                        cancelButton
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

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

        await dialog.ShowDialog(owner!);
        return await tcs.Task;
    }
    
    private async void ChangeRole_Click(object? sender, RoutedEventArgs _)
    {
        try
        {
            if (DataContext is not HomeViewModel vm || vm.UsersVm.SelectedUser == null)
                return;

            var selectedUser = vm.UsersVm.SelectedUser;

            var newRole = await ShowChangeRoleDialog(
                selectedUser.Username,
                selectedUser.Role.ToString()
            );

            if (string.IsNullOrWhiteSpace(newRole))
                return;

            await vm.UsersVm.ChangeRoleAsync(newRole);
        }
        catch (Exception ex)
        {
            await ShowErrorDialog(ex.ToString());
        }
    }
    
    private async Task ShowErrorDialog(string message)
    {
        var okButton = new Button
        {
            Content = "OK",
            Width = 90,
            Height = 36,
            Classes = { "dialogConfirmBtn" }
        };

        var owner = VisualRoot as Window;

        var dialog = new Window
        {
            Width = 500,
            Height = 300,
            CanResize = false,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20),
                BoxShadow = BoxShadows.Parse("0 10 30 0 #14000000"),
                Child = new StackPanel
                {
                    Spacing = 12,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Error",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = Brushes.Black
                        },
                        new SelectableTextBlock
                        {
                            Text = message,
                            Foreground = Brushes.Black,
                            TextWrapping = TextWrapping.Wrap
                        },
                        okButton
                    }
                }
            }
        };

        okButton.Click += (_, _) => dialog.Close();

        await dialog.ShowDialog(owner!);
    }
    
    private async void Inventory_Click(object? sender, RoutedEventArgs _)
    {
        try
        {
            if (DataContext is not HomeViewModel vm)
                return;

            var items = await vm.UsersVm.GetInventoryAsync();

            if (items == null)
                return;

            await ShowInventoryDialog(items);
        }
        catch (Exception ex)
        {
            await ShowErrorDialog(ex.ToString());
        }
    }
    
    private async Task ShowInventoryDialog(ItemCollectionDTO items)
    {
        var owner = VisualRoot as Window;

        var panel = new StackPanel { Spacing = 10 };

        panel.Children.Add(new TextBlock
        {
            Text = "Weapons",
            FontSize = 16,
            FontWeight = FontWeight.SemiBold
        });

        foreach (var w in items.Weapons)
        {
            panel.Children.Add(new TextBlock
            {
                Text = $"{w.Name} ({w.WeaponType}) | Cut: {w.Cut} | Blunt: {w.Blunt}",
                Foreground = Brushes.Black
            });
        }

        panel.Children.Add(new TextBlock
        {
            Text = "Armors",
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            Margin = new Thickness(0, 10, 0, 0)
        });

        foreach (var a in items.Armors)
        {
            panel.Children.Add(new TextBlock
            {
                Text = $"{a.Name} ({a.ArmorType}) | CutRes: {a.CutResistance} | BluntRes: {a.BluntResistance}",
                Foreground = Brushes.Black
            });
        }

        var dialog = new Window
        {
            Width = 500,
            Height = 500,
            CanResize = false,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20),
                BoxShadow = BoxShadows.Parse("0 10 30 0 #14000000"),
                Child = new ScrollViewer
                {
                    Content = panel
                }
            }
        };

        await dialog.ShowDialog(owner!);
    }
    
    private async void Equipment_Click(object? sender, RoutedEventArgs _)
    {
        try
        {
            if (DataContext is not HomeViewModel vm)
                return;

            var eq = await vm.UsersVm.GetEquipmentAsync();

            if (eq == null)
                return;

            await ShowEquipmentDialog(eq);
        }
        catch (Exception ex)
        {
            await ShowErrorDialog(ex.ToString());
        }
    }
    
    private async Task ShowEquipmentDialog(EquipmentResponseDTO eq)
    {
        var owner = VisualRoot as Window;

        var panel = new StackPanel { Spacing = 8 };

        void Add(string label, string? value)
        {
            panel.Children.Add(new TextBlock
            {
                Text = $"{label}: {value ?? "-"}",
                Foreground = Brushes.Black
            });
        }

        Add("Head", eq.Head?.Name);
        Add("Body", eq.Body?.Name);
        Add("Gloves", eq.Gloves?.Name);
        Add("Legs", eq.Legs?.Name);
        Add("Boots", eq.Boots?.Name);

        panel.Children.Add(new TextBlock
        {
            Text = "Weapons",
            FontWeight = FontWeight.SemiBold,
            Margin = new Thickness(0, 10, 0, 0)
        });

        Add("Weapon1", eq.Weapon1?.Name);
        Add("Weapon2", eq.Weapon2?.Name);
        Add("Weapon3", eq.Weapon3?.Name);
        Add("Weapon4", eq.Weapon4?.Name);

        var dialog = new Window
        {
            Width = 400,
            Height = 400,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20),
                BoxShadow = BoxShadows.Parse("0 10 30 0 #14000000"),
                Child = panel
            }
        };

        await dialog.ShowDialog(owner!);
    }
}