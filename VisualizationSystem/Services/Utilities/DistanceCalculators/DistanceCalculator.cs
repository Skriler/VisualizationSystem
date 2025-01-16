using VisualizationSystem.Models.Configs.DistanceMetrics;
using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.Domain.Nodes.Parameters;
using VisualizationSystem.Services.Utilities.DistanceCalculators.CategoricalMetrics;
using VisualizationSystem.Services.Utilities.DistanceCalculators.NumericMetrics;

namespace VisualizationSystem.Services.Utilities.DistanceCalculators;

public class DistanceCalculator : IDistanceCalculator
{
    private INumericDistanceMetric numericDistanceMetric = default!;
    private ICategoricalDistanceMetric categoricalDistanceMetric = default!;

    public void Initialize(DistanceMetricsConfig config)
    {
        numericDistanceMetric = config.NumericDistanceMetric;
        categoricalDistanceMetric = config.CategoricalDistanceMetric;
    }

    public double Calculate(CalculationNode firstNode, CalculationNode secondNode)
    {
        if (numericDistanceMetric == null || categoricalDistanceMetric == null)
            throw new InvalidOperationException("DistanceCalculator has not been initialized.");

        if (firstNode.Parameters.Count != secondNode.Parameters.Count)
            throw new ArgumentException("The nodes must have the same number of parameters.");

        var firstNumericValues = ExtractNumericValues(firstNode);
        var secondNumericValues = ExtractNumericValues(secondNode);

        var firstCategoricalParams = ExtractCategoricalParameters(firstNode);
        var secondCategoricalParams = ExtractCategoricalParameters(secondNode);

        var numericDistance = CalculateNumericDistance(firstNumericValues, secondNumericValues);
        var categoricalDistance = CalculateCategoricalDistance(firstCategoricalParams, secondCategoricalParams);

        return CalculateAverageDistance(
            numericDistance,
            firstNumericValues.Count,
            categoricalDistance,
            firstCategoricalParams.Count
            );
    }

    private List<double> ExtractNumericValues(CalculationNode node)
    {
        return node.Parameters
            .OfType<NumericParameter>()
            .Select(p => p.Value)
            .ToList();
    }

    private List<CategoricalParameter> ExtractCategoricalParameters(CalculationNode node)
    {
        return node.Parameters
            .OfType<CategoricalParameter>()
            .ToList();
    }

    private double CalculateNumericDistance(List<double> firstNumericValues, List<double> secondNumericValues)
    {
        if (firstNumericValues.Count == 0 || secondNumericValues.Count == 0)
            return 0;

        return numericDistanceMetric.CalculateDistance(firstNumericValues, secondNumericValues);
    }

    private double CalculateCategoricalDistance(
        List<CategoricalParameter> firstCategoricalParams,
        List<CategoricalParameter> secondCategoricalParams
        )
    {
        if (firstCategoricalParams.Count == 0 || secondCategoricalParams.Count == 0)
            return 0;

        var totalCategoricalDistance = 0d;
        for (int i = 0; i < firstCategoricalParams.Count; ++i)
        {
            var firstParam = firstCategoricalParams[i];
            var secondParam = secondCategoricalParams[i];

            if (firstParam == null || secondParam == null)
                continue;

            totalCategoricalDistance += categoricalDistanceMetric.CalculateDistance(
                firstParam.OneHotIndexes,
                secondParam.OneHotIndexes,
                firstParam.CategoryCount
                );
        }

        return totalCategoricalDistance / firstCategoricalParams.Count;
    }

    private double CalculateAverageDistance(
        double numericDistance,
        int numericParamsCount,
        double categoricalDistance,
        int categoricalParamsCount
        )
    {
        var weightedNumericDistance = numericDistance * numericParamsCount;
        var weightedCategoricalDistance = categoricalDistance * categoricalParamsCount;
        var totalCount = numericParamsCount + categoricalParamsCount;

        return (weightedNumericDistance + weightedCategoricalDistance) / totalCount;
    }
}
