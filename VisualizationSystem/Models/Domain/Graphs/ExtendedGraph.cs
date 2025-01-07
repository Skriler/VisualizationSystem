using Microsoft.Msagl.Drawing;
using VisualizationSystem.Models.DTOs;

namespace VisualizationSystem.Models.Domain.Graphs;

public class ExtendedGraph : Graph
{
    public string Type { get; private set; }

    public Dictionary<string, NodeSimilarityResult> NodeDataMap { get; set; } = new();

    public ExtendedGraph(string name, string type)
        : base(name)
    {
        Type = type;
    }
}
