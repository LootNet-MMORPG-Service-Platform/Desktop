using System;
using System.Collections.Generic;
using desktop_app.Enums;

namespace desktop_app.Models.Generation;

public class GenerationElement
{
    public Guid Id { get; set; }
    public ItemElementType ElementType { get; set; }
    public bool HasSegments => Segments.Count > 0;
    public List<Segment> Segments { get; set; } = new();
}
