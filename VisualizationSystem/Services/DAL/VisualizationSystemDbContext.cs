using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.Utilities.Clusterers;

namespace VisualizationSystem.Services.DAL;

public sealed class VisualizationSystemDbContext : DbContext
{
    public DbSet<NodeTable> NodeTables { get; set; }

    public DbSet<NodeObject> NodeObjects { get; set; }

    public DbSet<ParameterType> ParameterTypes { get; set; }
    
    public DbSet<NodeParameter> NodeParameters { get; set; }

    public DbSet<UserSettings> UserSettings { get; set; }

    public DbSet<ParameterState> ParameterStates { get; set; }

    public DbSet<ClusterAlgorithmSettings> ClusterAlgorithmSettings { get; set; }

    public VisualizationSystemDbContext(DbContextOptions<VisualizationSystemDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}
