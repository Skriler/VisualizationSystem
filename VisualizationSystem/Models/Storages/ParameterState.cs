using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.Storages;

public class ParameterState
{
    public ParameterType ParameterType { get; set; }
    public bool IsActive { get; set; }

    public ParameterState(ParameterType parameterType, bool isActive)
    {
        ParameterType = parameterType;
        IsActive = isActive;
    }
}
