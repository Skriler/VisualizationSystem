namespace VisualizationSystem.Models.Domain.Nodes.Parameters;

public class CategoricalParameter : BaseParameter
{
    public HashSet<int> OneHotIndexes { get; set; }

    public int CategoryCount { get; }

    public CategoricalParameter(IEnumerable<int> oneHotIndexes, int categoryCount)
    {
        OneHotIndexes = new HashSet<int>(oneHotIndexes);
        CategoryCount = categoryCount;
    }

    public override BaseParameter Clone() => new CategoricalParameter(OneHotIndexes, CategoryCount);
}
