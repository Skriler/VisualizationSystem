using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities.Nodes.Normalized;

public class NormNode
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int NodeObjectId { get; set; }
    public NodeObject NodeObject { get; set; } = default!;

    [Required]
    public int NodeTableId { get; set; }
    public NodeTable NodeTable { get; set; } = default!;

    public List<NormParameter> NormParameters { get; set; } = new();
}
