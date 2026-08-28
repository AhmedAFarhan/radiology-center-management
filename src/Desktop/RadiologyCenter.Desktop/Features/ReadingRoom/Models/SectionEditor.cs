namespace RadiologyCenter.Desktop.Features.ReadingRoom.Models;

public sealed class SectionEditor
{
    public string Type { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int Position { get; set; }
    public bool Locked { get; set; }
    public bool Exists { get; set; }
}
