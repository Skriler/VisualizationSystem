using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities.Nodes;

public class NormalizedParameterState
{
    [Key]
    public int Id { get; set; }

    [Required]
    public double Weight { get; set; }

    [Required]
    public int ParameterTypeId { get; set; }
    public ParameterType ParameterType { get; set; } = default!;
}
