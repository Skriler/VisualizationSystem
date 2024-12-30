using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities.Nodes;

public class NormalizedParameter
{
    [Key]
    public int Id { get; set; }

    [Required]
    public double Value { get; set; }

    [Required]
    public int NodeObjectId { get; set; }
    public NodeObject NodeObject { get; set; } = default!;

    [Required]
    public int NormalizedParameterStateId { get; set; }
    public NormalizedParameterState NormalizedParameterState { get; set; } = default!;
}
