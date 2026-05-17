using System;
using desktop_app.Enums;

namespace desktop_app.Models;

public class NotificationMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public NotificationType Type { get; set; }
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";

    public string AccentBrushKey => Type switch
    {
        NotificationType.Success => "SuccessBrush",
        NotificationType.Error => "DangerBrush",
        NotificationType.Warning => "PrColor",
        _ => "InfoBrush"
    };
}
