namespace VisualizationSystem.Services.Utilities.DistanceCalculators.CategoricalMetrics;

public class HammingDistanceMetric : ICategoricalDistanceMetric
{
    public double CalculateDistance(int[] firstValues, int[] secondValues)
    {
        int differences = 0;

        for (int i = 0; i < firstValues.Length; ++i)
        {
            if (firstValues[i] == secondValues[i])
                continue;

            ++differences;
        }

        return (double)differences / firstValues.Length;
    }
}
