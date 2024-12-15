using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Storages.Nodes;

namespace VisualizationSystem.Models.Storages.Results;

public class NodeSimilarityResult
{
    public NodeObject Node { get; }
    public List<SimilarNode> SimilarNodes { get; } = new();
    public int SimilarNodesAboveThreshold { get; private set; }

    public NodeSimilarityResult(NodeObject node)
    {
        Node = node;
    }

    public void UpdateSimilarNodesAboveThreshold(float minSimilarityPercentage)
    {
        SimilarNodesAboveThreshold = SimilarNodes
            .Count(sn => sn.SimilarityPercentage > minSimilarityPercentage);
    }
}
