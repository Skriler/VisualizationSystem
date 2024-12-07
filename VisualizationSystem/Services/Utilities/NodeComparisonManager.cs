using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.Services.Utilities.Clusterers;
using VisualizationSystem.Services.Utilities.Comparers;

namespace VisualizationSystem.Services.Utilities;

public class NodeComparisonManager
{
    private readonly ClustererFactory clustererFactory;
    private readonly ICompare comparer;

    private IClusterize clusterer;

    public List<NodeSimilarityResult> SimilarityResults { get; private set; }
    public List<Cluster> Clusters { get; private set; }
    public UserSettings Settings { get; private set; }

    public NodeComparisonManager(ClustererFactory clustererFactory, ICompare comparer)
    {
        this.clustererFactory = clustererFactory;
        this.comparer = comparer;
    }

    public void UpdateSettings(UserSettings settings)
    {
        Settings = settings;
        clusterer = clustererFactory.CreateClusterer(Settings.ClusterAlgorithm);
    }

    public void CalculateSimilarNodes(NodeTable table)
    {
        var similarityResultsDict = table.NodeObjects
            .ToDictionary(node => node, node => new NodeSimilarityResult(node));

        var nodes = table.NodeObjects;

        for (int i = 0; i < nodes.Count; ++i)
        {
            var firstNode = nodes[i];

            for (int j = i + 1; j < nodes.Count; ++j)
            {
                var secondNode = nodes[j];

                var similarityPercentage = GetSimilarityPercentage(firstNode, secondNode);

                similarityResultsDict[firstNode]
                    .SimilarNodes
                    .Add(new SimilarNode(secondNode, similarityPercentage));

                similarityResultsDict[secondNode]
                    .SimilarNodes
                    .Add(new SimilarNode(firstNode, similarityPercentage));
            }
        }

        SimilarityResults = similarityResultsDict.Values.ToList();

        SimilarityResults.ForEach(similarityResult =>
        {
            similarityResult.UpdateSimilarNodesAboveThreshold(Settings.MinSimilarityPercentage);
        });
    }

    public void CalculateClusters(float minSimilarityThreshold)
    {
        if (SimilarityResults.Count <= 0)
            throw new InvalidOperationException("Node analysis must be performed first before calculating clusters.");

        Clusters = clusterer.Cluster(SimilarityResults, minSimilarityThreshold);
    }

    private float GetSimilarityPercentage(NodeObject firstNode, NodeObject secondNode)
    {
        if (firstNode.Parameters.Count != secondNode.Parameters.Count)
            throw new InvalidOperationException("NodeObjects must have the same number of parameters");

        float totalMatchedWeight = 0;
        float totalActiveWeight = 0;

        var activeParameterStates = Settings.GetActiveParameters();
        var firstNodeParameters = firstNode.Parameters.ToDictionary(p => p.ParameterType);
        var secondNodeParameters = secondNode.Parameters.ToDictionary(p => p.ParameterType);

        foreach (var parameterState in activeParameterStates)
        {
            totalActiveWeight += parameterState.Weight;

            firstNodeParameters.TryGetValue(parameterState.ParameterType, out var firstParameter);
            secondNodeParameters.TryGetValue(parameterState.ParameterType, out var secondParameter);

            if (firstParameter == null || secondParameter == null)
                continue;

            if (firstParameter.ParameterType.Name != secondParameter.ParameterType.Name)
                continue;

            if (!comparer.Compare(firstParameter.Value, secondParameter.Value, Settings.DeviationPercent))
                continue;

            totalMatchedWeight += parameterState.Weight;
        }

        return totalActiveWeight > 0 ? (totalMatchedWeight / totalActiveWeight) * 100 : 0;

    }
}
