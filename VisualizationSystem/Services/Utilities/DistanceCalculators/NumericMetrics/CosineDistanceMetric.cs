namespace VisualizationSystem.Services.Utilities.DistanceCalculators.NumericMetrics;

public class CosineDistanceMetric : INumericDistanceMetric
{
    public double CalculateDistance(List<double> firstValues, List<double> secondValues)
    {
        if (firstValues.Count != secondValues.Count)
            throw new ArgumentException("Lists must be the same length");

        var dotProduct = firstValues
            .Zip(secondValues, (x, y) => x * y)
            .Sum();

        var firstMagnitude = firstValues
            .Sum(x => x * x);

        var secondMagnitude = secondValues
            .Sum(y => y * y);

        if (firstMagnitude == 0 || secondMagnitude == 0)
            return 1;

        var cosineSimilarity = dotProduct / (Math.Sqrt(firstMagnitude) * Math.Sqrt(secondMagnitude));

        return 1 - cosineSimilarity;
    }
}
