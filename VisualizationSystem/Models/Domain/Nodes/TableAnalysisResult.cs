using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Models.Domain.Nodes;

public class TableAnalysisResult
{
    public string Name { get; }

    public List<NodeObject> Nodes { get; }

    public List<NodeSimilarityResult> SimilarityResults { get; set; } = new();

    public List<Cluster> Clusters { get; set; } = new();

    public TableAnalysisResult(NodeTable table)
    {
        Name = table.Name;
        Nodes = table.NodeObjects;
    }
}
