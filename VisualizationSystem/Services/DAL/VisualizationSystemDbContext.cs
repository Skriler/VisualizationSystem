using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Normalized;
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

        ConfigureTablePerHierarchy(modelBuilder);
        ConfigureCascadeDelete(modelBuilder);
    }

    private void ConfigureTablePerHierarchy(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NormalizedParameter>()
            .ToTable("NormalizedParameters")
            .HasDiscriminator<string>("ParameterType")
            .HasValue<NormalizedNumericParameter>("Numeric")
            .HasValue<NormalizedCategoricalParameter>("Categorical");
    }

    private void ConfigureCascadeDelete(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NodeTable>(entity =>
        {
            entity.HasMany(e => e.NodeObjects)
                .WithOne(node => node.NodeTable)
                .HasForeignKey(node => node.NodeTableId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.ParameterTypes)
                .WithOne(type => type.NodeTable)
                .HasForeignKey(type => type.NodeTableId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<NodeObject>(entity =>
        {
            entity.HasMany(e => e.Parameters)
                .WithOne(parameter => parameter.NodeObject)
                .HasForeignKey(parameter => parameter.NodeObjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.NormalizedParameters)
                .WithOne(parameter => parameter.NodeObject)
                .HasForeignKey(parameter => parameter.NodeObjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.HasOne(e => e.NodeTable)
                .WithMany()
                .HasForeignKey(e => e.NodeTableId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AlgorithmSettings)
                .WithOne(algorithm => algorithm.UserSettings)
                .HasForeignKey<ClusterAlgorithmSettings>(algorithm => algorithm.UserSettingsId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<NodeParameter>()
            .HasOne(parameter => parameter.ParameterType)
            .WithMany()
            .HasForeignKey(parameter => parameter.ParameterTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<NormalizedParameter>()
            .HasOne(parameter => parameter.NormalizedParameterState)
            .WithMany()
            .HasForeignKey(parameter => parameter.NormalizedParameterStateId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<NormalizedParameterState>()
            .HasOne(state => state.ParameterType)
            .WithMany()
            .HasForeignKey(state => state.ParameterTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ParameterState>()
            .HasOne(state => state.UserSettings)
            .WithMany(settings => settings.ParameterStates)
            .HasForeignKey(state => state.UserSettingsId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ParameterState>()
            .HasOne(state => state.ParameterType)
            .WithOne()
            .HasForeignKey<ParameterState>(state => state.ParameterTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
