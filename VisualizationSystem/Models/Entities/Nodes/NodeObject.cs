using System.ComponentModel.DataAnnotations;
using VisualizationSystem.Models.Entities.Normalized;

namespace VisualizationSystem.Models.Entities.Nodes;

public class NodeObject
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int NodeTableId { get; set; }
    public NodeTable NodeTable { get; set; } = default!;

    public List<NodeParameter> Parameters { get; set; } = new();

    public List<NormalizedParameter> NormalizedParameters { get; set; } = new();

    public override int GetHashCode() => Name.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is NodeObject otherNode)
            return Name == otherNode.Name;

        return false;
    }
}
