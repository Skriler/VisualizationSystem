﻿using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities.Nodes;

public class NodeObject
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public List<NodeParameter> Parameters { get; set; } = new();
}
