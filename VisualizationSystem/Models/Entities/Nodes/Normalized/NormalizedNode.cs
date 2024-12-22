using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities.Nodes.Normalized;

public class NormalizedNode
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int NodeObjectId { get; set; }
    public NodeObject NodeObject { get; set; } = default!;

    [Required]
    public int NodeTableId { get; set; }
    public NodeTable NodeTable { get; set; } = default!;

    public List<NormalizedNodeParameter> NormalizedParameters { get; set; } = new();
}
