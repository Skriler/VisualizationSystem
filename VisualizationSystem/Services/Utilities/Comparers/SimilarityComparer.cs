using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.Utilities.Settings;

namespace VisualizationSystem.Services.Utilities.Comparers;

public class SimilarityComparer : ISettingsObserver
{
    private readonly ICompare comparer;

    private UserSettings settings = default!;
    private Dictionary<NodeObject, NodeSimilarityResult> similarityResultsDict = new();

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
        similarityResultsDict = nodes
            .ToDictionary(node => node, node => new NodeSimilarityResult(node));

        CompareNodes(nodes);

        return similarityResultsDict.Values.ToList();
    }

    private void CompareNodes(List<NodeObject> nodes)
    {
        for (int i = 0; i < nodes.Count; ++i)
        {
            var firstNode = nodes[i];

            for (int j = i + 1; j < nodes.Count; ++j)
            {
                var secondNode = nodes[j];
                var similarityPercentage = GetSimilarityPercentage(firstNode, secondNode);

                AddSimilarNode(firstNode, secondNode, similarityPercentage);
                AddSimilarNode(secondNode, firstNode, similarityPercentage);
            }
        }
    }

    private float GetSimilarityPercentage(NodeObject firstNode, NodeObject secondNode)
    {
        if (firstNode.Parameters.Count != secondNode.Parameters.Count)
            throw new InvalidOperationException("NodeObjects must have the same number of parameters");

        var totalMatchedWeight = 0f;
        var totalActiveWeight = 0f;

        var activeParameterStates = settings.ParameterStates
            .Where(p => p.IsActive)
            .ToList();

        foreach (var parameterState in activeParameterStates)
        {
            totalActiveWeight += parameterState.Weight;

            if (!TryGetParameterByState(firstNode, parameterState, out var firstParameter))
                continue;

            if (!TryGetParameterByState(secondNode, parameterState, out var secondParameter))
                continue;

            if (!comparer.Compare(firstParameter.Value, secondParameter.Value, settings.DeviationPercent))
                continue;

            totalMatchedWeight += parameterState.Weight;
        }

        return CalculateSimilarityPercentage(totalMatchedWeight, totalActiveWeight);
    }

    private void AddSimilarNode(NodeObject source, NodeObject target, float similarityPercentage)
    {
        similarityResultsDict[source].SimilarNodes
            .Add(new SimilarNode(target, similarityPercentage));
    }

    private static bool TryGetParameterByState(NodeObject node, ParameterState state, out NodeParameter parameter)
    {
        parameter = node.Parameters
            .First(p => p.ParameterType.Name == state.ParameterType.Name);

        return parameter != null;
    }

    private static float CalculateSimilarityPercentage(float totalMatchedWeight, float totalActiveWeight)
    {
        return totalActiveWeight > 0 ? (totalMatchedWeight / totalActiveWeight) * 100 : 0;
    }
}
