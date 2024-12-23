using Microsoft.Msagl.Drawing;
using VisualizationSystem.Models.Storages.Results;

namespace VisualizationSystem.Models.Storages.Graphs;

public class ExtendedGraph : Graph
{
    public Dictionary<string, NodeSimilarityResult> NodeDataMap { get; set; } = new();

    public ExtendedGraph(string name) 
        : base(name) 
    { }
}
