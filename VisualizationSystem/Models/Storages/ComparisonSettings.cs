using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.Storages;

public class ComparisonSettings
{
    private readonly int DefaultMinMatchingParameters = 40;
    private readonly int DefaultDeviationPercent = 10;

    public int MinMatchingParameters { get; set; }
    public float DeviationPercent { get; set; }
    public List<ParameterState> ParameterStates { get; set; } = new List<ParameterState>();

    public ComparisonSettings()
    {
        SetDefaultSettings();
    }

    public void SetDefaultSettings()
    {
        MinMatchingParameters = DefaultMinMatchingParameters;
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
