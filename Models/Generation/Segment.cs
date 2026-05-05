using System;

namespace desktop_app.Models.Generation;

public class Segment
{
    public Guid Id { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public double Weight { get; set; }
}