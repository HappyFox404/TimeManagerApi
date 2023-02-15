namespace TimeManagerApi.Core.Context.Entity;

public class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime DateCreated { get; set; } = DateTime.Now;
}