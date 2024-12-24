using Microsoft.Msagl.Drawing;
using VisualizationSystem.Models.DTOs;

namespace VisualizationSystem.Models.Domain.Graphs;

public class ExtendedGraph : Graph
{
    public Dictionary<string, NodeSimilarityResult> NodeDataMap { get; set; } = new();

    public ExtendedGraph(string name)
        : base(name)
    { }
}
