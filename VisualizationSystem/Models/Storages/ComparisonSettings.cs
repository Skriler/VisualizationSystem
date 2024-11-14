using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.Storages;

public class ComparisonSettings
{
    private readonly float DefaultMinSimilarityPercentage = 55;
    private readonly float DefaultDeviationPercent = 10;

    public float MinSimilarityPercentage { get; set; }
    public float DeviationPercent { get; set; }
    public List<ParameterState> ParameterStates { get; set; } = new List<ParameterState>();

    public ComparisonSettings()
    {
        SetDefaultSettings();
    }

    public void SetDefaultSettings()
    {
        MinSimilarityPercentage = DefaultMinSimilarityPercentage;
        DeviationPercent = DefaultDeviationPercent;
        ParameterStates = new List<ParameterState>();
    }

    public void InitializeParameterStatuses(List<ParameterType> parameterTypes)
    {
        ParameterStates = parameterTypes
            .Select(name => new ParameterState(name, true))
            .ToList();
    }

    public List<ParameterState> GetActiveParameters()
    {
        return ParameterStates.Where(p => p.IsActive).ToList();
    }
}
