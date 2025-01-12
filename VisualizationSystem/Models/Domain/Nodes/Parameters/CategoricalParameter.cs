namespace VisualizationSystem.Models.Domain.Nodes.Parameters;

public class CategoricalParameter : BaseParameter
{
    public HashSet<int> OneHotIndexes { get; }

    public int CategoryCount { get; }

    public CategoricalParameter(IEnumerable<int> oneHotIndexes, int categoryCount, int mergeCount = 1)
        : base(mergeCount)
    {
        OneHotIndexes = new HashSet<int>(oneHotIndexes);
        CategoryCount = categoryCount;
    }

    public override BaseParameter Clone() => new CategoricalParameter(OneHotIndexes, CategoryCount);

    public override BaseParameter Merge(BaseParameter other)
    {
        if (other is not CategoricalParameter categorical)
            throw new ArgumentException("Cannot merge CategoricalParameter with non-CategoricalParameter");

        var mergedIndexes = new HashSet<int>(OneHotIndexes);
        mergedIndexes.UnionWith(categorical.OneHotIndexes);

        return new CategoricalParameter(mergedIndexes, CategoryCount + other.MergeCount);
    }
}
