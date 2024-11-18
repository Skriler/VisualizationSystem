using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities;

public class NodeTable
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public List<ParameterType> ParameterTypes { get; set; } = new();

    public List<NodeObject> NodeObjects { get; set; } = new();
}
