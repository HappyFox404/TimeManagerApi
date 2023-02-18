namespace TimeManagerApi.Core.Context.Entity;

public class Schedule : BaseEntity
{
    public DateTime Day { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
}