using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.Utilities.Settings;

namespace VisualizationSystem.Services.Utilities.Comparers;

public class SimilarityComparer : ISettingsObserver
{
    private readonly ICompare comparer;

    private UserSettings settings;

    public SimilarityComparer(
        ICompare comparer,
        ISettingsSubject settingsSubject
        )
    {
        this.comparer = comparer;

        settingsSubject.Attach(this);
    }

    public void Update(UserSettings settings) => this.settings = settings;

    public List<NodeSimilarityResult> CalculateSimilarNodes(List<NodeObject> nodes)
    {
        var similarityResultsDict = nodes
            .ToDictionary(node => node, node => new NodeSimilarityResult(node));

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

        var similarityResults = similarityResultsDict.Values.ToList();

        similarityResults.ForEach(sr =>
        {
            sr.SimilarNodesAboveThreshold = sr.SimilarNodes
                .Count(sn => sn.SimilarityPercentage > settings.MinSimilarityPercentage);
        });

        return similarityResults;
    }

    private float GetSimilarityPercentage(NodeObject firstNode, NodeObject secondNode)
    {
        if (firstNode.Parameters.Count != secondNode.Parameters.Count)
            throw new InvalidOperationException("NodeObjects must have the same number of parameters");

        float totalMatchedWeight = 0;
        float totalActiveWeight = 0;

        var activeParameterStates = settings.GetActiveParameters();
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

            if (!comparer.Compare(firstParameter.Value, secondParameter.Value, settings.DeviationPercent))
                continue;

            totalMatchedWeight += parameterState.Weight;
        }

        return totalActiveWeight > 0 ? totalMatchedWeight / totalActiveWeight * 100 : 0;

    }
}
