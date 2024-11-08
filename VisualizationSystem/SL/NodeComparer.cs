﻿using VisualizationSystem.Models.Entities;
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
                    GetSimilarParametersCount(firstNode, secondNode)
                );

                comparisonResults.Add(comparisonResult);
            }
        }

        return comparisonResults;
    }

    private int GetSimilarParametersCount(NodeObject firstNode, NodeObject secondNode)
    {
        if (firstNode.Parameters.Count != secondNode.Parameters.Count)
            throw new InvalidOperationException("NodeObjects must have the same number of parameters");

        int similarCount = 0;

        NodeParameter firstParameter, secondParameter;
        for (int i = 0; i < firstNode.Parameters.Count; ++i)
        {
            firstParameter = firstNode.Parameters[i];
            secondParameter = secondNode.Parameters[i];

            if (firstParameter.ParameterType.Id != secondParameter.ParameterType.Id)
                continue;

            if (!IsParameterMatch(firstParameter.Value, secondParameter.Value))
                continue;

            ++similarCount;
        }

        return similarCount;
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
