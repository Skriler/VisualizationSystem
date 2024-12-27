using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities.Nodes.Normalized;

public class NormParameterState
{
    [Key]
    public int Id { get; set; }

    [Required]
    public double Weight { get; set; }
}
