using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Normalized;
using VisualizationSystem.Services.Utilities.DistanceCalculators.Categorical;
using VisualizationSystem.Services.Utilities.DistanceCalculators.Numeric;

namespace VisualizationSystem.Services.Utilities.DistanceCalculators;

public class MetricDistanceCalculator
{
    private readonly INumericDistance numericDistance;
    private readonly ICategoricalDistance categoricalDistance;

    public MetricDistanceCalculator(INumericDistance numericDistance, ICategoricalDistance categoricalDistance)
    {
        this.numericDistance = numericDistance;
        this.categoricalDistance = categoricalDistance;
    }

    public double CalculateDistance(NodeObject firstNode, NodeObject secondNode)
    {
        if (firstNode.NormalizedParameters.Count != secondNode.NormalizedParameters.Count)
            throw new ArgumentException("The nodes must have the same number of parameters.");

        var firstNumericValues = ExtractNumericValues(firstNode);
        var secondNumericValues = ExtractNumericValues(secondNode);

        var firstCategoricalParams = ExtractCategoricalParameters(firstNode);
        var secondCategoricalParams = ExtractCategoricalParameters(secondNode);

        var numericDistanceValue = CalculateNumericDistance(firstNumericValues, secondNumericValues);
        var categoricalDistanceValue = CalculateCategoricalDistance(firstCategoricalParams, secondCategoricalParams);

        int totalParamsCount = firstNumericValues.Count + firstCategoricalParams.Count;

        if (totalParamsCount == 0)
            return 0;

        double weightedNumericDistance = numericDistanceValue * firstNumericValues.Count;
        double weightedCategoricalDistance = categoricalDistanceValue * firstCategoricalParams.Count;

        return (weightedNumericDistance + weightedCategoricalDistance) / totalParamsCount;
    }

    private List<double> ExtractNumericValues(NodeObject node)
    {
        return node.NormalizedParameters
            .OfType<NormalizedNumericParameter>()
            .Select(p => p.Value)
            .ToList();
    }

    private List<NormalizedCategoricalParameter> ExtractCategoricalParameters(NodeObject node)
    {
        return node.NormalizedParameters
            .OfType<NormalizedCategoricalParameter>()
            .ToList();
    }

    private double CalculateNumericDistance(List<double> firstNumericValues, List<double> secondNumericValues)
    {
        if (firstNumericValues.Count == 0 || secondNumericValues.Count == 0)
            return 0;

        return numericDistance.CalculateDistance(firstNumericValues, secondNumericValues);
    }

    private double CalculateCategoricalDistance(
        List<NormalizedCategoricalParameter> firstCategoricalParams,
        List<NormalizedCategoricalParameter> secondCategoricalParams
        )
    {
        var totalCategoricalDistance = 0d;

        for (int i = 0; i < firstCategoricalParams.Count; ++i)
        {
            var firstParam = firstCategoricalParams[i];
            var secondParam = secondCategoricalParams[i];

            if (firstParam == null || secondParam == null)
                continue;

            totalCategoricalDistance += categoricalDistance.CalculateDistance(
                firstParam.OneHotIndexes,
                secondParam.OneHotIndexes,
                firstParam.NormalizedParameterState.CategoryCount
                );
        }

        return totalCategoricalDistance / firstCategoricalParams.Count;
    }
}
