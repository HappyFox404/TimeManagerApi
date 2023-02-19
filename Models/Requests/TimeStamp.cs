namespace TimeManagerApi.Models.Requests;

public class TimeStampCreate
{
    public DateTime Time { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsNotify { get; set; }
}

public class TimeStampEdit : TimeStampCreate
{
    public Guid TimeStampId { get; set; }
}

public class TimeStampCreateFromSchedule : TimeStampCreate
{
    public Guid ScheduleId { get; set; }
}

public class TimeStampCreateWithSchedule : TimeStampCreate
{
    public DateTime Day { get; set; }
}