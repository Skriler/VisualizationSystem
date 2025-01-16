namespace VisualizationSystem.Services.Utilities.DistanceCalculators.NumericMetrics;

public class ManhattanDistanceMetric : INumericDistanceMetric
{
    public double CalculateDistance(List<double> firstValues, List<double> secondValues)
    {
        if (firstValues.Count != secondValues.Count)
            throw new ArgumentException("Lists must be the same length");

        var distance = 0d;
        for (int i = 0; i < firstValues.Count; i++)
        {
            distance += Math.Abs(firstValues[i] - secondValues[i]);
        }

        return distance / firstValues.Count;
    }
}
