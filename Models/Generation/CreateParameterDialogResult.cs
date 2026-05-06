using System.Collections.Generic;
using desktop_app.Enums;

namespace desktop_app.Models.Generation;

public class CreateParameterDialogResult
{
    public ItemParameter Parameter { get; set; }

    public List<CreateSegmentInput> Segments { get; set; } = new();
}