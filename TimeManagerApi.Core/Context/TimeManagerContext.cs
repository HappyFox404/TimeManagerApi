﻿using Microsoft.EntityFrameworkCore;
using TimeManagerApi.Core.Context.Entity;

namespace TimeManagerApi.Core.Context;

public class TimeManagerContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public TimeManagerContext(DbContextOptions<TimeManagerContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(x => x.Id);
        modelBuilder.Entity<User>().Property(x => x.UserName).IsRequired();
        modelBuilder.Entity<User>().Property(x => x.UserName).HasMaxLength(50);
        modelBuilder.Entity<User>().Property(x => x.Password).IsRequired();
        modelBuilder.Entity<User>().Property(x => x.Password).HasMaxLength(50);
        modelBuilder.Entity<User>().Property(x => x.Email).IsRequired();
        modelBuilder.Entity<User>().Property(x => x.Email).HasMaxLength(100);
        modelBuilder.Entity<User>().HasIndex(x => x.UserName).IsUnique();
    }
}