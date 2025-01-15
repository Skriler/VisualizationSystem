namespace VisualizationSystem.Models.Domain.Nodes.Parameters;

public class NumericParameter : BaseParameter
{
    public double Value { get; set; }

    public NumericParameter(double value)
    {
        Value = value;
    }

    public override BaseParameter Clone() => new NumericParameter(Value);
}
