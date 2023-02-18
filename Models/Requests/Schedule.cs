namespace TimeManagerApi.Models.Requests;

public class ScheduleEdit
{
    public Guid Id { get; set; }
    public DateTime Day { get; set; }
}

public class ScheduleCreate
{
    public DateTime Day { get; set; }
}