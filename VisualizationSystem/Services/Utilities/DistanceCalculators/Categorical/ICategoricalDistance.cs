namespace VisualizationSystem.Services.Utilities.DistanceCalculators.Categorical;

public interface ICategoricalDistance
{
    double CalculateDistance(List<int> firstValues, List<int> secondValues, int categoriesCount);
}
