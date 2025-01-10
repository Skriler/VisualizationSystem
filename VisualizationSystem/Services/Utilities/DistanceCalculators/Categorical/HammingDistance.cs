namespace VisualizationSystem.Services.Utilities.DistanceCalculators.Categorical;

public class HammingDistance : ICategoricalDistance
{
    public double CalculateDistance(List<int> firstIndexes, List<int> secondIndexes, int categoriesCount)
    {
        var firstSet = new HashSet<int>(firstIndexes);
        var secondSet = new HashSet<int>(secondIndexes);

        var differences = firstSet.Except(secondSet).Count()
            + secondSet.Except(firstSet).Count();

        return differences / (double)categoriesCount;
    }
}
