using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace desktop_app.Services.Dialogs.Generation;

public static class ProfileGenerationDialogs
{
    public static async Task<string?> ShowCreateProfileDialogAsync(Window owner)
    {
        var tcs = new TaskCompletionSource<string?>();

        var textBox = new TextBox
        {
            Width = 220,
            Watermark = "Profile name"
        };

        var createButton = GenerationDialogUiFactory.CreateDialogButton("Create", "detailsBtn");
        var cancelButton = GenerationDialogUiFactory.CreateDialogButton("Cancel", "dialogCancelBtn");
        var errorText = new TextBlock
        {
            Foreground = Brushes.IndianRed,
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            IsVisible = false
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
                            Foreground = GenerationDialogUiFactory.GetBrush("TextPrimaryBrush", Brushes.Black),
                            TextAlignment = TextAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },

                        textBox,
                        errorText,

                        GenerationDialogUiFactory.CreateButtonRow(createButton, cancelButton)
                    }
                }
            }
        };

        var dialog = GenerationDialogUiFactory.CreateBaseDialog(content, 360, 205);
        dialog.Title = "Create profile";

        cancelButton.Click += (_, _) =>
        {
            tcs.TrySetResult(null);
            dialog.Close();
        };

        createButton.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                errorText.Text = "Name is required.";
                errorText.IsVisible = true;
                return;
            }

            if (textBox.Text.Trim().Length > 80)
            {
                errorText.Text = "Name must be between 1 and 80 characters.";
                errorText.IsVisible = true;
                return;
            }

            tcs.TrySetResult(textBox.Text);
            dialog.Close();
        };

        await dialog.ShowDialog(owner);
        return await tcs.Task;
    }
}
