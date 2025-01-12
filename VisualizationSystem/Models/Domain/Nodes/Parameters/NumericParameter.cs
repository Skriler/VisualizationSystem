namespace VisualizationSystem.Models.Domain.Nodes.Parameters;

public class NumericParameter : BaseParameter
{
    public double Value { get; }

    public NumericParameter(double value, int mergeCount = 1)
        : base(mergeCount)
    {
        Value = value;
    }

    public override BaseParameter Clone() => new NumericParameter(Value);

    public override BaseParameter Merge(BaseParameter other)
    {
        if (other is not NumericParameter numeric)
            throw new ArgumentException("Cannot merge NumericParameter with non-NumericParameter");

        var weightedSum = Value * MergeCount + numeric.Value * numeric.MergeCount;
        var totalCount = MergeCount + numeric.MergeCount;
        var newValue = weightedSum / totalCount;

        return new NumericParameter(newValue, totalCount);
    }
}
