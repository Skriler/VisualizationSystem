using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.Storages;

public class NodeComparisonResult
{
    public NodeObject FirstNode { get; private set; }
    public NodeObject SecondNode { get; private set; }
    public int SimilarityCount { get; private set; }

    public NodeComparisonResult(NodeObject firstNode, NodeObject secondNode, int similarityCount)
    {
        FirstNode = firstNode;
        SecondNode = secondNode;
        SimilarityCount = similarityCount;
    }
}
