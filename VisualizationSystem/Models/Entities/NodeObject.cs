using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities;

public class NodeObject
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    public List<NodeParameter> Parameters  { get; set; } = new List<NodeParameter>();
}
