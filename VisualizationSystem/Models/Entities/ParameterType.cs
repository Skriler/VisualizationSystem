using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities;

public class ParameterType
{
    private static readonly bool DefaultIsActive = true;
    private static readonly float DefaultWeight = 1;

    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [Required]
    public float Weight { get; set; }

    public ParameterType(string name)
    {
        Name = name;
        ResetToDefaults();
    }

    public void ResetToDefaults()
    {
        IsActive = DefaultIsActive;
        Weight = DefaultWeight;
    }
}
