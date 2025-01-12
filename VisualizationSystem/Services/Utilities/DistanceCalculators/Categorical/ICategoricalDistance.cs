namespace VisualizationSystem.Services.Utilities.DistanceCalculators.Categorical;

public interface ICategoricalDistance
{
    double CalculateDistance(IEnumerable<int> firstValues, IEnumerable<int> secondValues, int categoriesCount);
}
