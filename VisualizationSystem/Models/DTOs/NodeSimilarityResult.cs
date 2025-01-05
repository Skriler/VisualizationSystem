using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Models.DTOs;

public class NodeSimilarityResult
{
    public NodeObject Node { get; }

    public List<SimilarNode> SimilarNodes { get; } = new();

    public NodeSimilarityResult(NodeObject node)
    {
        Node = node;
    }
}
