using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities;

public class NodeParameter
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Value { get; set; }

    [Required]
    public int NodeObjectId { get; set; }
    public NodeObject NodeObject { get; set; } = default!;
}
