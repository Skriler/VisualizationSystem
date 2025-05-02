namespace VisualizationSystem.Models.Entities.Normalized;

public class NormalizedCategoricalParameter : NormalizedParameter
{
    public int[] OneHotIndexes { get; set; } = default!;
}
