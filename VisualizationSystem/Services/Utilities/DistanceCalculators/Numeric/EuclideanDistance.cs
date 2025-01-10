namespace VisualizationSystem.Services.Utilities.DistanceCalculators.Numeric;

public class EuclideanDistance : INumericDistance
{
    public double CalculateDistance(List<double> firstValues, List<double> secondValues)
    {
        if (firstValues.Count != secondValues.Count)
            throw new ArgumentException("Lists must be the same length");

        var distanceSquared = 0d;
        for (int i = 0; i < firstValues.Count; ++i)
        {
            distanceSquared += Math.Pow(firstValues[i] - secondValues[i], 2);
        }

        var distance = Math.Sqrt(distanceSquared);
        return distance / Math.Sqrt(firstValues.Count);
    }
}
