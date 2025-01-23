using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Normalized;

namespace VisualizationSystem.Services.Utilities.Normalizers;

public class NumericNormalizer : ITypeNormalizer
{
    public int Id { get; }

    public ParameterValueType Type { get; } = ParameterValueType.Numeric;

    public int CategoryCount => 1;

    private double Min { get; set; }

    private double Max { get; set; }

    private double Average { get; set; }

    public NumericNormalizer(int id, double value)
    {
        Id = id;
        Min = Max = Average = value;
    }

    public void AddValue(string value)
    {
        if (!double.TryParse(value, out var numericValue))
            throw new ArgumentException($"Invalid numeric value: {value}");

        Min = Math.Min(Min, numericValue);
        Max = Math.Max(Max, numericValue);
        Average = (Min + Max) / 2;
    }

    public NormalizedParameter CreateNormalizedParameter(
        NodeParameter parameter,
        NodeObject node,
        NormalizedParameterState state
        )
    {
        var value = parameter.Value != string.Empty
            ? Convert.ToDouble(parameter.Value)
            : Average;

        return new NormalizedNumericParameter
        {
            Value = NormalizeMinMax(value),
            NodeObjectId = node.Id,
            NormalizedParameterStateId = state.Id
        };
    }

    private double NormalizeMinMax(double value)
    {
        if (Max == Min)
            return 1;

        return (value - Min) / (Max - Min);
    }
}
