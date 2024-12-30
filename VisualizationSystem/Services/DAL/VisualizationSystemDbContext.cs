using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;

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

    public DbSet<NormalizedParameterState> NormalizedParameterStates { get; set; }

    public DbSet<NormalizedParameter> NormalizedParameters { get; set; }

    public VisualizationSystemDbContext(DbContextOptions<VisualizationSystemDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureCascadeDeleteForNodeTable(modelBuilder);
    }

    private void ConfigureCascadeDeleteForNodeTable(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NodeTable>(entity =>
        {
            entity.HasMany(e => e.NodeObjects)
                  .WithOne(no => no.NodeTable)
                  .HasForeignKey(no => no.NodeTableId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.ParameterTypes)
                  .WithOne(pt => pt.NodeTable)
                  .HasForeignKey(no => no.NodeTableId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<NodeObject>(entity =>
        {
            entity.HasMany(e => e.Parameters)
                  .WithOne(np => np.NodeObject)
                  .HasForeignKey(np => np.NodeObjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.NormalizedParameters)
                  .WithOne(np => np.NodeObject)
                  .HasForeignKey(np => np.NodeObjectId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<NodeParameter>()
            .HasOne(np => np.ParameterType)
            .WithMany()
            .HasForeignKey(np => np.ParameterTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserSettings>()
            .HasOne(us => us.NodeTable)
            .WithMany()
            .HasForeignKey(us => us.NodeTableId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ParameterState>()
            .HasOne(ps => ps.UserSettings)
            .WithMany(us => us.ParameterStates)
            .HasForeignKey(ps => ps.UserSettingsId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ClusterAlgorithmSettings>()
            .HasOne(cas => cas.UserSettings)
            .WithOne(us => us.AlgorithmSettings)
            .HasForeignKey<ClusterAlgorithmSettings>(cas => cas.UserSettingsId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<NormalizedParameterState>()
            .HasOne(nps => nps.ParameterType)
            .WithMany()
            .HasForeignKey(nps => nps.ParameterTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<NormalizedParameter>()
            .HasOne(np => np.NormalizedParameterState)
            .WithMany()
            .HasForeignKey(np => np.NormalizedParameterStateId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
