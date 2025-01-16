namespace VisualizationSystem.Services.Utilities.DistanceCalculators.CategoricalMetrics;

public class HammingDistanceMetric : ICategoricalDistanceMetric
{
    public double CalculateDistance(IEnumerable<int> firstIndexes, IEnumerable<int> secondIndexes, int categoriesCount)
    {
        var differences = firstIndexes.Except(secondIndexes).Count()
            + secondIndexes.Except(firstIndexes).Count();

        return differences / (double)categoriesCount;
    }
}
