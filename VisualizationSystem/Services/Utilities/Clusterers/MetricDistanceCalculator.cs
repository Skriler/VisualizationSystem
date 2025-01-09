using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.Entities.Normalized;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class MetricDistanceCalculator
{
    public double CalculateEuclidean(List<NormalizedNumericParameter> firstParameters, List<NormalizedNumericParameter> secondParameters)
    {
        var secondValues = secondParameters
            .ConvertAll(p => p.Value);

        return CalculateWeightedEuclidean(firstParameters, secondValues);
    }

    public double CalculateEuclidean(List<NormalizedNumericParameter> firstParameters, List<double> secondParameters)
    {
        return CalculateWeightedEuclidean(firstParameters, secondParameters);
    }

    private double CalculateWeightedEuclidean(List<NormalizedNumericParameter> parameters, List<double> values)
    {
        if (parameters.Count != values.Count)
            throw new InvalidOperationException("Parameters must be the same length");

        double distance = 0;

        for (int i = 0; i < parameters.Count; ++i)
        {
            var diff = parameters[i].Value - values[i];
            distance += diff * diff;
        }

        return Math.Sqrt(distance);
    }

    public double CalculateCosine(List<WeightedParameter> firstParameters, List<WeightedParameter> secondParameters)
    {
        if (firstParameters.Count != secondParameters.Count)
            throw new InvalidOperationException("Parameters must be the same length");

        var dotProduct = firstParameters
            .Zip(secondParameters, (x, y) => x.Value * y.Value * x.Weight)
            .Sum();

        var firstMagnitude = firstParameters
            .Sum(x => x.Value * x.Value * x.Weight);

        var secondMagnitude = secondParameters
            .Sum(y => y.Value * y.Value * y.Weight);

        if (firstMagnitude == 0 || secondMagnitude == 0)
            return 0;

        return dotProduct / (Math.Sqrt(firstMagnitude) * Math.Sqrt(secondMagnitude));
    }
}
