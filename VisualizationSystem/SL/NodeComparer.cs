using Microsoft.VisualBasic;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;

namespace VisualizationSystem.SL;

public class NodeComparer
{
    private ComparisonSettings settings;

    public NodeComparer(ComparisonSettings comparisonSettings)
    {
        settings = comparisonSettings;
    }

    public void UpdateSettings(ComparisonSettings comparisonSettings) => settings = comparisonSettings;

    public List<NodeComparisonResult> GetSimilarNodes(NodeTable table)
    {
        var comparisonResults = new List<NodeComparisonResult>();

        var nodes = table.NodeObjects;

        NodeObject firstNode, secondNode;
        NodeComparisonResult comparisonResult;

        for (int i = 0; i < nodes.Count; ++i)
        {
            for (int j = i + 1; j < nodes.Count; ++j)
            {
                firstNode = nodes[i];
                secondNode = nodes[j];

                comparisonResult = new NodeComparisonResult(
                    firstNode,
                    secondNode,
                    GetSimilarityPercentage(firstNode, secondNode)
                );

                comparisonResults.Add(comparisonResult);
            }
        }

        return comparisonResults;
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

            if (firstParameter.ParameterType.Id != secondParameter.ParameterType.Id)
                continue;

            if (!IsParameterMatch(firstParameter.Value, secondParameter.Value))
                continue;

            totalMatchedWeight += parameterState.Weight;
        }

        return totalActiveWeight > 0 ? (totalMatchedWeight / totalActiveWeight) * 100 : 0;

    }

    private bool IsParameterMatch(string firstValue, string secondValue)
    {
        if (string.IsNullOrWhiteSpace(firstValue) || string.IsNullOrWhiteSpace(secondValue))
            return false;

        if (double.TryParse(firstValue, out double firstNumber) && 
            double.TryParse(secondValue, out double secondNumber))
        {
            double maxNumber = Math.Max(firstNumber, secondNumber);
            double tolerance = maxNumber * settings.DeviationPercent / 100;

            return Math.Abs(firstNumber - secondNumber) <= tolerance;
        }

        return firstValue == secondValue;
    }
}
