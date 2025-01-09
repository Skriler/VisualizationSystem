using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Normalized;

namespace VisualizationSystem.Services.Utilities.Normalizers;

public class CategoricalNormalizer : ITypeNormalizer
{
    public int Id { get; }

    public ParameterValueType Type { get; } = ParameterValueType.Categorical;

    public int CategoryCount => Values.Count;

    public List<string> Values { get; private set; } = new();

    public CategoricalNormalizer(int id, string value)
    {
        Id = id;
        AddValue(value);
    }

    public void AddValue(string value)
    {
        if (Values.Contains(value))
            return;

        Values.Add(value);
    }

    public NormalizedParameter CreateNormalizedParameter(
        NodeParameter parameter,
        NodeObject node,
        NormalizedParameterState state
        )
    {
        var oneHotArray = ConvertStringToOneHot(parameter.Value);
        var indices = oneHotArray
            .Select((value, index) => new { value, index })
            .Where(x => x.value == 1)
            .Select(x => x.index)
            .ToList();

        return new NormalizedCategoricalParameter
        {
            OneHotIndexes = indices,
            NodeObject = node,
            NormalizedParameterState = state
        };
    }

    private int[] ConvertStringToOneHot(string value)
    {
        var encoded = new int[Values.Count];
        var index = Values.IndexOf(value);

        if (index != -1)
        {
            encoded[index] = 1;
        }

        return encoded;
    }
}
