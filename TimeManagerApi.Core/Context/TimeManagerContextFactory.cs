using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TimeManagerApi.Core.Context;

public class FamilyMoviesLibraryContextFactory : IDesignTimeDbContextFactory<TimeManagerContext>
{
    public TimeManagerContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TimeManagerContext>();
        //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TimeManager;Username=postgres;Password=12345rvs");
        //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TimeManager;Username=postgres;Password=123QWE!@#");
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TimeManager;Username=admin;Password=123QWE!@#");
        return new TimeManagerContext(optionsBuilder.Options);
    }
}