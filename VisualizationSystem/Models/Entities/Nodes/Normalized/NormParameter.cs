using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities.Nodes.Normalized;

public class NormParameter
{
    [Key]
    public int Id { get; set; }

    [Required]
    public double Value { get; set; }

    [Required]
    public int NormNodeId { get; set; }
    public NormNode NormNode { get; set; } = default!;

    [Required]
    public int NormParameterStateId { get; set; }
    public NormParameterState NormParameterState { get; set; } = default!;
}
