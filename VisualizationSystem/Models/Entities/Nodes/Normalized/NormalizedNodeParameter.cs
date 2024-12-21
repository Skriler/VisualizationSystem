using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities.Nodes.Normalized;

public class NormalizedNodeParameter
{
    [Key]
    public int Id { get; set; }

    [Required]
    public double Value { get; set; }

    [Required]
    public int ParameterTypeId { get; set; }
    public ParameterType ParameterType { get; set; } = default!;
}
