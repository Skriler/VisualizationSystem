using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.Storages;

public class ParameterState
{
    private static readonly bool DefaultIsActive = true;
    private static readonly float DefaultWeight = 1;

    public ParameterType ParameterType { get; set; }
    public bool IsActive { get; set; }
    public float Weight { get; set; }

    public ParameterState(ParameterType parameterType)
    {
        ParameterType = parameterType;
        ResetToDefaults();
    }

    public void ResetToDefaults()
    {
        IsActive = DefaultIsActive;
        Weight = DefaultWeight;
    }
}
