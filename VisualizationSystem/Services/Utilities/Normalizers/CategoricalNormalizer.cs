using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Normalized;

namespace VisualizationSystem.Services.Utilities.Normalizers;

public class CategoricalNormalizer : ITypeNormalizer
{
    public int Id { get; }

    public ParameterValueType Type { get; } = ParameterValueType.Categorical;

    public int CategoryCount => Categories.Count;

    public List<string> Categories { get; private set; } = new();

    public CategoricalNormalizer(int id, string value)
    {
        Id = id;
        AddValue(value);
    }

    public void AddValue(string originalValue)
    {
        var values = GetSplitValues(originalValue);

        foreach (var value in values)
        {
            if (Categories.Contains(value))
                continue;

            Categories.Add(value);
        }
    }

    public NormalizedParameter CreateNormalizedParameter(
        NodeParameter parameter,
        NodeObject node,
        NormalizedParameterState state
        )
    {
        var oneHotArray = ConvertValueToOneHot(parameter.Value);
        var indices = oneHotArray
            .Select((value, index) => new { value, index })
            .Where(x => x.value == 1)
            .Select(x => x.index)
            .ToList();

        return new NormalizedCategoricalParameter
        {
            OneHotIndexes = indices,
            NodeObjectId = node.Id,
            NormalizedParameterStateId = state.Id
        };
    }

    private int[] ConvertValueToOneHot(string originalValue)
    {
        var encoded = new int[Categories.Count];

        if (originalValue == string.Empty)
            return encoded;

        var values = GetSplitValues(originalValue);

        foreach (var value in values)
        {
            var index = Categories.IndexOf(value);
            encoded[index] = 1;
        }

        return encoded;
    }
    private static List<string> GetSplitValues(string values)
    {
        return values.Split(',')
            .Select(v => v.Trim())
            .Order()
            .ToList();
    }
}
