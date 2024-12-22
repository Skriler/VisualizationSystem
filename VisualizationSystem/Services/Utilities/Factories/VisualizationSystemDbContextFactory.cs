using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using VisualizationSystem.Services.DAL;

namespace VisualizationSystem.Services.Utilities.Factories;

public class VisualizationSystemDbContextFactory : IDesignTimeDbContextFactory<VisualizationSystemDbContext>
{
    public VisualizationSystemDbContext CreateDbContext(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();

        var optionsBuilder = new DbContextOptionsBuilder<VisualizationSystemDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        return new VisualizationSystemDbContext(optionsBuilder.Options);
    }
}
