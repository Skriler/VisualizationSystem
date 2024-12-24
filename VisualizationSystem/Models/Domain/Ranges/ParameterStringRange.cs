namespace VisualizationSystem.Models.Domain.Ranges;

public class ParameterStringRange
{
    public int Id { get; private set; }
    public List<string> Values { get; private set; } = new();

    public ParameterStringRange(int id)
    {
        Id = id;
    }

    public void AddValue(string value)
    {
        if (Values.Contains(value))
            return;

        Values.Add(value);
    }
}
