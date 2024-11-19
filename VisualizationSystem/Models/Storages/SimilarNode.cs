using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.Storages;
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
