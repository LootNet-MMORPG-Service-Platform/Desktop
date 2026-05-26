using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using desktop_app.Models;

namespace desktop_app.Services;

public static class CommonDialogService
{
    private static IBrush GetBrush(string key, IBrush fallback)
    {
        return DialogWindowFactory.GetBrush(key, fallback);
    }

    private static Window CreateBaseDialog(Control content, double width, double height)
    {
        return DialogWindowFactory.CreateBaseDialog(content, width, height);
    }

    public static async Task<bool> ShowConfirmDialogAsync(
        Window owner,
        string message,
        string confirmText = "Confirm")
    {
        _ = confirmText;

        var tcs = new TaskCompletionSource<bool>();

        var confirmButton = new Button
        {
            Content = "Confirm",
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
        dialog.Title = "Error";

        okButton.Click += (_, _) => dialog.Close();

        await dialog.ShowDialog(owner);
    }

    public static async Task ShowReadonlyTextDialogAsync(Window owner, string title, string text)
    {
        var closeButton = new Button
        {
            Content = "Close",
            Width = 90,
            Height = 36,
            Classes = { "dialogCancelBtn" }
        };

        var content = new StackPanel
        {
            Spacing = 12,
            Children =
            {
                new TextBlock
                {
                    Text = title,
                    FontSize = 16,
                    FontWeight = FontWeight.SemiBold,
                    Foreground = GetBrush("TextPrimaryBrush", Brushes.Black)
                },
                new ScrollViewer
                {
                    Height = 330,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Content = new SelectableTextBlock
                    {
                        Text = text,
                        TextWrapping = TextWrapping.Wrap,
                        FontFamily = new FontFamily("Consolas"),
                        Foreground = GetBrush("TextPrimaryBrush", Brushes.Black)
                    }
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Children =
                    {
                        closeButton
                    }
                }
            }
        };

        var dialog = CreateBaseDialog(content, 700, 480);
        dialog.Title = title;

        closeButton.Click += (_, _) => dialog.Close();

        await dialog.ShowDialog(owner);
    }

    public static async Task<ChangePasswordDialogResult?> ShowChangePasswordDialogAsync(Window owner)
    {
        var tcs = new TaskCompletionSource<ChangePasswordDialogResult?>();

        var oldPasswordBox = new TextBox
        {
            Watermark = "Old password",
            PasswordChar = '*',
            Width = 260
        };

        var newPasswordBox = new TextBox
        {
            Watermark = "New password",
            PasswordChar = '*',
            Width = 260
        };

        var confirmPasswordBox = new TextBox
        {
            Watermark = "Confirm new password",
            PasswordChar = '*',
            Width = 260
        };

        var errorText = new TextBlock
        {
            Foreground = Brushes.IndianRed,
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            IsVisible = false
        };

        var saveButton = new Button
        {
            Content = "Save",
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
                    Width = 280,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Change password",
                            FontSize = 16,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },
                        oldPasswordBox,
                        newPasswordBox,
                        confirmPasswordBox,
                        errorText,
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

        var dialog = CreateBaseDialog(content, 400, 335);
        dialog.Title = "Change password";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        saveButton.Click += (_, _) =>
        {
            var oldPassword = oldPasswordBox.Text ?? "";
            var newPassword = newPasswordBox.Text ?? "";
            var confirmPassword = confirmPasswordBox.Text ?? "";

            if (string.IsNullOrWhiteSpace(oldPassword))
            {
                ShowValidationError(errorText, "Old password is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ShowValidationError(errorText, "New password is required.");
                return;
            }

            if (newPassword != confirmPassword)
            {
                ShowValidationError(errorText, "Confirm password must match.");
                return;
            }

            tcs.TrySetResult(new ChangePasswordDialogResult
            {
                OldPassword = oldPassword,
                NewPassword = newPassword
            });
            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }

    private static void ShowValidationError(TextBlock errorText, string message)
    {
        errorText.Text = message;
        errorText.IsVisible = true;
    }
}
