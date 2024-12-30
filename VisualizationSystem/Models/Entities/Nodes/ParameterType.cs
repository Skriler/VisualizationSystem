using System.ComponentModel.DataAnnotations;
using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Models.Entities;

public class ParameterType
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public int NodeTableId { get; set; }
    public NodeTable NodeTable { get; set; } = default!;
}
