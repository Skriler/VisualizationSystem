using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Nodes.Normalized;
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

    public DbSet<NormNode> NormNodes { get; set; }

    public DbSet<NormParameterState> NormParameterState { get; set; }

    public DbSet<NormParameter> NormNodeParameters { get; set; }

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
        modelBuilder.Entity<NodeTable>()
            .HasMany(nt => nt.NodeObjects)
            .WithOne()
            .HasForeignKey("NodeTableId")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NodeObject>()
            .HasMany(no => no.Parameters)
            .WithOne(np => np.NodeObject)
            .HasForeignKey(np => np.NodeObjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NodeParameter>()
            .HasOne(np => np.ParameterType)
            .WithMany()
            .HasForeignKey(np => np.ParameterTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserSettings>()
            .HasOne(us => us.NodeTable)
            .WithMany()
            .HasForeignKey(us => us.NodeTableId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ParameterState>()
            .HasOne(ps => ps.UserSettings)
            .WithMany(us => us.ParameterStates)
            .HasForeignKey(ps => ps.UserSettingsId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClusterAlgorithmSettings>()
            .HasOne(cas => cas.UserSettings)
            .WithOne(us => us.AlgorithmSettings)
            .HasForeignKey<ClusterAlgorithmSettings>(cas => cas.UserSettingsId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NormNode>()
            .HasOne(nn => nn.NodeTable)
            .WithMany()
            .HasForeignKey(nn => nn.NodeTableId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<NormNode>()
            .HasOne(nn => nn.NodeObject)
            .WithMany() 
            .HasForeignKey(nn => nn.NodeObjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NormNode>()
            .HasMany(nn => nn.NormParameters)
            .WithOne(nnp => nnp.NormNode)
            .HasForeignKey(nnp => nnp.NormNodeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NormParameter>()
            .HasOne(np => np.NormParameterState)
            .WithMany()
            .HasForeignKey(nnp => nnp.NormNodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
