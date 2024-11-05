using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities;

public class NodeParameter
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Value { get; set; } = string.Empty;

    [Required]
    public int NodeObjectId { get; set; }
    public NodeObject NodeObject { get; set; } = default!;

    [Required]
    public int ParameterTypeId { get; set; }
    public ParameterType ParameterType { get; set; } = default!;
}
