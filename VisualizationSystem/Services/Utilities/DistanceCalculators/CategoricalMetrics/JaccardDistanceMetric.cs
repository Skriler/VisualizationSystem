namespace VisualizationSystem.Services.Utilities.DistanceCalculators.CategoricalMetrics;

public class JaccardDistanceMetric : ICategoricalDistanceMetric
{
    public double CalculateDistance(int[] firstValues, int[] secondValues)
    {
        int intersection = 0;
        int union = 0;

        for (int i = 0; i < firstValues.Length; ++i)
        {
            if (firstValues[i] == 1 && secondValues[i] == 1)
                ++intersection;

            if (firstValues[i] == 1 || secondValues[i] == 1)
                ++union;
        }

        return union == 0 ? 0 : 1.0 - (double)intersection / union;
    }
}
