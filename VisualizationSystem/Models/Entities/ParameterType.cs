using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities;

public class ParameterType
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
}
