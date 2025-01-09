namespace VisualizationSystem.Models.Entities.Normalized;

public class NormalizedCategoricalParameter : NormalizedParameter
{
    public List<int> OneHotIndexes { get; set; } = new();
}
