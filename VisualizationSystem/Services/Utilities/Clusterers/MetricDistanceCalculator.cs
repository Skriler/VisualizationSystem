using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class MetricDistanceCalculator
{
    public double CalculateEuclidean(List<NormalizedParameter> firstParameters, List<NormalizedParameter> secondParameters)
    {
        var secondValues = secondParameters.Select(p => p.Value).ToList();

        return CalculateWeightedEuclidean(firstParameters, secondValues);
    }

    public double CalculateEuclidean(List<NormalizedParameter> firstParameters, List<double> secondParameters)
    {
        return CalculateWeightedEuclidean(firstParameters, secondParameters);
    }

    private double CalculateWeightedEuclidean(List<NormalizedParameter> parameters, List<double> values)
    {
        if (parameters.Count != values.Count)
            throw new InvalidOperationException("Parameters must be the same length");

        double distance = 0;

        for (int i = 0; i < parameters.Count; ++i)
        {
            var diff = parameters[i].Value - values[i];
            distance += (diff * diff) * parameters[i].NormalizedParameterState.Weight;
        }

        return Math.Sqrt(distance);
    }

    public double CalculateCosine(List<double> firstParameters, List<double> secondParameters)
    {
        if (firstParameters.Count != secondParameters.Count)
            throw new InvalidOperationException("Parameters must be the same length");

        var dotProduct = firstParameters.Zip(secondParameters, (x, y) => x * y).Sum();
        var firstMagnitude = Math.Sqrt(firstParameters.Sum(x => x * x));
        var secondMagnitude = Math.Sqrt(secondParameters.Sum(x => x * x));

        return (float)(dotProduct / (firstMagnitude * secondMagnitude));
    }
}
