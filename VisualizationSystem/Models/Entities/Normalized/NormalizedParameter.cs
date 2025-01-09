using System.ComponentModel.DataAnnotations;
using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Models.Entities.Normalized;

public class NormalizedParameter
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int NodeObjectId { get; set; }
    public NodeObject NodeObject { get; set; } = default!;

    [Required]
    public int NormalizedParameterStateId { get; set; }
    public NormalizedParameterState NormalizedParameterState { get; set; } = default!;
}
