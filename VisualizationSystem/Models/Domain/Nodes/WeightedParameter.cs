
namespace VisualizationSystem.Models.Domain.Nodes;

public class WeightedParameter
{
    public double Value { get; set; }

    public double Weight { get; set; }

    public WeightedParameter(double value, double weight)
    {
        Value = value;
        Weight = weight;
    }
}
