using System.Text.Json.Serialization;

namespace TimeManagerApi.Core.Context.Entity;

public class TimeStamp : BaseEntity
{
    public DateTime Time { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsNotify { get; set; } = true;
    public Guid ScheduleId { get; set; }
    [JsonIgnore] public virtual Schedule Schedule { get; set; } = null!;
}