using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Threading;
using desktop_app.Enums;
using desktop_app.Models;

namespace desktop_app.Services;

public class NotificationService
{
    private const int DefaultDismissDelayMs = 4500;
    private static readonly Lazy<NotificationService> LazyInstance = new(() => new NotificationService());

    public static NotificationService Instance => LazyInstance.Value;

    public ObservableCollection<NotificationMessage> Notifications { get; } = new();

    public void ShowSuccess(string message, string title = "Success") =>
        Show(NotificationType.Success, title, message);

    public void ShowError(string message, string title = "Error") =>
        Show(NotificationType.Error, title, message);

    public void ShowWarning(string message, string title = "Warning") =>
        Show(NotificationType.Warning, title, message);

    public void ShowInfo(string message, string title = "Info") =>
        Show(NotificationType.Info, title, message);

    public void ShowApiError(Exception exception, string fallbackMessage = "Operation failed.")
    {
        if (exception is HttpRequestException { StatusCode: null })
        {
            ShowError("API unavailable. Check if the server is running.");
            return;
        }

        ShowError(fallbackMessage);
    }

    private void Show(NotificationType type, string title, string message)
    {
        var notification = new NotificationMessage
        {
            Type = type,
            Title = title,
            Message = message
        };

        Dispatcher.UIThread.Post(() =>
        {
            if (Notifications.Any(n => n.Title == title && n.Message == message))
                return;

            Notifications.Add(notification);
            _ = DismissLaterAsync(notification);
        });
    }

    private async Task DismissLaterAsync(NotificationMessage notification)
    {
        await Task.Delay(DefaultDismissDelayMs);
        Dispatcher.UIThread.Post(() => Notifications.Remove(notification));
    }
}
