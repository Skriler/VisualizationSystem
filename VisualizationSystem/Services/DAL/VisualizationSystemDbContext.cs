using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Services.DAL;

public class VisualizationSystemDbContext : DbContext
{
    public DbSet<NodeTable> NodeTable { get; set; }

    public DbSet<NodeObject> NodeObjects { get; set; }

    public DbSet<ParameterType> ParameterTypes { get; set; }
    
    public DbSet<NodeParameter> NodeParameters { get; set; }

    public DbSet<UserSettings> UserSettings { get; set; }

    public VisualizationSystemDbContext(DbContextOptions<VisualizationSystemDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}
