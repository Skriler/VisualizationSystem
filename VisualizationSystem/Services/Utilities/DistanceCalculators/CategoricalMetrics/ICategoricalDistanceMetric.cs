namespace VisualizationSystem.Services.Utilities.DistanceCalculators.CategoricalMetrics;

public interface ICategoricalDistanceMetric
{
    double CalculateDistance(IEnumerable<int> firstValues, IEnumerable<int> secondValues, int categoriesCount);
}
