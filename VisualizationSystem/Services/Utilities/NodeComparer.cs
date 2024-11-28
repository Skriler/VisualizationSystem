using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.Services.Utilities.Comparers;

namespace VisualizationSystem.Services.Utilities;

public class NodeComparer
{
    private readonly ICompare comparer;

    public UserSettings Settings { get; private set; }

    public NodeComparer(ICompare comparer)
    {
        this.comparer = comparer;
    }

    public void UpdateSettings(UserSettings settings) => Settings = settings;

    public List<NodeSimilarityResult> GetSimilarNodes(NodeTable table)
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

        var similarityResults = new List<NodeSimilarityResult>();

        similarityResultsDict.Values
            .ToList()
            .ForEach(similarityResult =>
            {
                similarityResult.UpdateSimilarNodesAboveThreshold(Settings.MinSimilarityPercentage);
                similarityResults.Add(similarityResult);
            });

        return similarityResults;
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

            if (firstParameter.ParameterType.Id != secondParameter.ParameterType.Id)
                continue;

            if (!comparer.Compare(firstParameter.Value, secondParameter.Value, Settings.DeviationPercent))
                continue;

            totalMatchedWeight += parameterState.Weight;
        }

        return totalActiveWeight > 0 ? (totalMatchedWeight / totalActiveWeight) * 100 : 0;

    }
}
