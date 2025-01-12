namespace VisualizationSystem.Services.Utilities.DistanceCalculators.Categorical;

public class HammingDistance : ICategoricalDistance
{
    public double CalculateDistance(IEnumerable<int> firstIndexes, IEnumerable<int> secondIndexes, int categoriesCount)
    {
        var differences = firstIndexes.Except(secondIndexes).Count()
            + secondIndexes.Except(firstIndexes).Count();

        return differences / (double)categoriesCount;
    }
}
