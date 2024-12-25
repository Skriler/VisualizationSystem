using VisualizationSystem.Models.Entities.Settings;

namespace VisualizationSystem.Models.Domain.Settings;

public class ParameterStateData
{
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public float Weight { get; set; }

    public ParameterStateData(ParameterState parameterState)
    {
        SetData(parameterState);
    }

    public void SetData(ParameterState parameterState)
    {
        Name = parameterState.ParameterType.Name;
        IsActive = parameterState.IsActive;
        Weight = parameterState.Weight;
    }
}
