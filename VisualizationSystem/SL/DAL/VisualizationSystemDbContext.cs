using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.SL.DAL;

public class VisualizationSystemDbContext : DbContext
{
    public DbSet<NodeObject> NodeObjects { get; set; }
    public DbSet<NodeParameter> NodeParameters { get; set; }

    public VisualizationSystemDbContext(DbContextOptions<VisualizationSystemDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    base.OnModelCreating(modelBuilder);

    //    modelBuilder.Entity<NodeParameter>()
    //        .HasOne(p => p.NodeObject)
    //        .WithMany(n => n.Parameters)
    //        .HasForeignKey(p => p.NodeObjectId);
            
    //}
}
