using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Models.Domain.Nodes;

public class SimilarNode
{
    public NodeObject Node { get; }
    public float SimilarityPercentage { get; }

    public SimilarNode(NodeObject node, float similarityPercentage)
    {
        Node = node;
        SimilarityPercentage = similarityPercentage;
    }
}
