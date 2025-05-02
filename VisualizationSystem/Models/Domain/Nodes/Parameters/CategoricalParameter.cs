namespace VisualizationSystem.Models.Domain.Nodes.Parameters;

public class CategoricalParameter : BaseParameter
{
    public int[] OneHotValues { get; set; }

    public int CategoryCount { get; }

    public CategoricalParameter(int[] oneHotValues)
    {
        OneHotValues = oneHotValues.ToArray();
    }

    public override BaseParameter Clone() => new CategoricalParameter(OneHotValues);
}
