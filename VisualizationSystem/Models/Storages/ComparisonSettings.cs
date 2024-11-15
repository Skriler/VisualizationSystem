using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.Storages;

public class ComparisonSettings
{
    private static readonly float DefaultMinSimilarityPercentage = 55f;
    private static readonly float DefaultDeviationPercent = 10f;

    public float MinSimilarityPercentage { get; set; }
    public float DeviationPercent { get; set; }
    public List<ParameterState> ParameterStates { get; set; } = new List<ParameterState>();

    public ComparisonSettings()
    {
        ResetCoreValues();
    }

    public void ResetToDefaults()
    {
        ResetCoreValues();
        ResetParameterStates();
    }

    public void ResetCoreValues()
    {
        MinSimilarityPercentage = DefaultMinSimilarityPercentage;
        DeviationPercent = DefaultDeviationPercent;
    }

    public void ResetParameterStates()
    {
        ParameterStates.ForEach(p => p.ResetToDefaults());
    }

    public void InitializeParameterStatuses(List<ParameterType> parameterTypes)
    {
        ParameterStates = parameterTypes
            .Select(name => new ParameterState(name))
            .ToList();
    }

    public List<ParameterState> GetActiveParameters()
    {
        return ParameterStates.Where(p => p.IsActive).ToList();
    }
}
