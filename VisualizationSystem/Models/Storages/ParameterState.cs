using System.Windows.Media.Media3D;
using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.Storages;

public class ParameterState
{
    public ParameterType ParameterType { get; set; }
    public bool IsActive { get; set; }
    public float Weight { get; set; }

    public ParameterState(ParameterType parameterType, bool isActive, float weight = 1)
    {
        ParameterType = parameterType;
        IsActive = isActive;
        Weight = weight;
    }
}
