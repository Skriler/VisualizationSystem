using System.ComponentModel.DataAnnotations;
using VisualizationSystem.Models.Domain.Settings;

namespace VisualizationSystem.Models.Entities.Settings;

public class ParameterState
{
    private const bool DefaultIsActive = true;
    private const float DefaultWeight = 1;

    [Key]
    public int Id { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [Required]
    public float Weight { get; set; }

    [Required]
    public int UserSettingsId { get; set; }
    public UserSettings UserSettings { get; set; } = default!;

    [Required]
    public int ParameterTypeId { get; set; }
    public ParameterType ParameterType { get; set; } = default!;

    public ParameterState()
    {
        ResetToDefaults();
    }

    public ParameterState(ParameterType parameterType, UserSettings userSettings)
    {
        ParameterType = parameterType;
        UserSettings = userSettings;
        ResetToDefaults();
    }

    public void ResetToDefaults()
    {
        IsActive = DefaultIsActive;
        Weight = DefaultWeight;
    }

    public void SetData(ParameterStateData parameterData)
    {
        IsActive = parameterData.IsActive;
        Weight = parameterData.Weight;
    }
}
