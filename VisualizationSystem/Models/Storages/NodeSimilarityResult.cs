using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.Storages;

public class NodeSimilarityResult
{
    public NodeObject Node { get; }
    public List<SimilarNode> SimilarNodes { get; } = new();

    public NodeSimilarityResult(NodeObject node)
    {
        Node = node;
    }
}
