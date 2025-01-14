namespace VisualizationSystem.Models.Domain.Nodes.Parameters;

public class CategoricalParameter : BaseParameter
{
    private const float MinCategoryThreshold = 0.3f;

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

        var totalCount = MergeCount + categorical.MergeCount;
        var threshold = totalCount * MinCategoryThreshold;

        foreach (var index in categorical.OneHotIndexes)
        {
            var count = OneHotIndexes.Contains(index) ? MergeCount : 0;
            count += categorical.OneHotIndexes.Contains(index) ? MergeCount : 0;

            if (count < threshold)
                continue;

            mergedIndexes.Add(index);
        }

        return new CategoricalParameter(mergedIndexes, CategoryCount);
    }
}
