using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.Storages;

public class NodeComparisonResult
{
    public NodeObject FirstNode { get; private set; }
    public NodeObject SecondNode { get; private set; }
    public float SimilarityPercentage { get; private set; }

    public NodeComparisonResult(NodeObject firstNode, NodeObject secondNode, float similarityPercentage)
    {
        FirstNode = firstNode;
        SecondNode = secondNode;
        SimilarityPercentage = similarityPercentage;
    }
}
