using Microsoft.EntityFrameworkCore;
using TimeManagerApi.Core.Context.Entity;

namespace TimeManagerApi.Core.Context;

public class TimeManagerContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<TimeStamp> TimeStamps { get; set; }

    public TimeManagerContext(DbContextOptions<TimeManagerContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(x => x.Id);
        modelBuilder.Entity<User>().Property(x => x.UserName).IsRequired();
        modelBuilder.Entity<User>().Property(x => x.UserName).HasMaxLength(50);
        modelBuilder.Entity<User>().Property(x => x.Password).IsRequired();
        modelBuilder.Entity<User>().Property(x => x.Email).IsRequired();
        modelBuilder.Entity<User>().Property(x => x.Email).HasMaxLength(100);
        modelBuilder.Entity<User>().HasIndex(x => x.UserName).IsUnique();
        
        modelBuilder.Entity<Schedule>().HasKey(x => x.Id);
        modelBuilder.Entity<Schedule>().Property(x => x.Day).IsRequired();

        modelBuilder.Entity<Schedule>()
            .HasOne(x => x.User)
            .WithMany(x => x.Schedules)
            .HasForeignKey(x => x.UserId)
            .HasPrincipalKey(x => x.Id);

        modelBuilder.Entity<TimeStamp>().HasKey(x => x.Id);
        modelBuilder.Entity<TimeStamp>().Property(x => x.Time).IsRequired();
        modelBuilder.Entity<TimeStamp>().Property(x => x.Title).IsRequired();
        modelBuilder.Entity<TimeStamp>().Property(x => x.Title).HasMaxLength(60);
        modelBuilder.Entity<TimeStamp>().Property(x => x.IsNotify).IsRequired();

        modelBuilder.Entity<TimeStamp>()
            .HasOne(x => x.Schedule)
            .WithMany(x => x.TimeStamps)
            .HasForeignKey(x => x.ScheduleId)
            .HasPrincipalKey(x => x.Id);
    }
}