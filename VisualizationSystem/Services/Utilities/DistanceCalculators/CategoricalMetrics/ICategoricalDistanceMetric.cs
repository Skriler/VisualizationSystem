namespace VisualizationSystem.Services.Utilities.DistanceCalculators.CategoricalMetrics;

public interface ICategoricalDistanceMetric
{
    double CalculateDistance(int[] firstValues, int[] secondValues);
}
