using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities.Normalized;

public class NormalizedParameterState
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CategoryCount { get; set; }

    [Required]
    public ParameterValueType ValueType { get; set; }

    [Required]
    public int ParameterTypeId { get; set; }
    public ParameterType ParameterType { get; set; } = default!;
}
