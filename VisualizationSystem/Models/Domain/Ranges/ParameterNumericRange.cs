namespace VisualizationSystem.Models.Domain.Ranges;

public class ParameterNumericRange
{
    public int Id { get; private set; }
    public double Min { get; private set; }
    public double Max { get; private set; }

    public ParameterNumericRange(int id, double min, double max)
    {
        Id = id;
        Update(min, max);
    }

    public void Update(double min, double max)
    {
        Min = min;
        Max = max;
    }
}
