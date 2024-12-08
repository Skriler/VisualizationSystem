namespace VisualizationSystem.Models.Storages;

public class ParameterRange
{
    public double Min { get; private set; }
    public double Max { get; private set; }

    public ParameterRange(double min, double max)
    {
        Min = min;
        Max = max;
    }

    public bool Contains(double value) => value >= Min && value <= Max;
}
