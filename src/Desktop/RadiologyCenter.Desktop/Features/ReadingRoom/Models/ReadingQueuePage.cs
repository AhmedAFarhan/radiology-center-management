namespace RadiologyCenter.Desktop.Features.ReadingRoom.Models;
public sealed class ReadingQueuePage<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int ImagedCount { get; set; }
}
