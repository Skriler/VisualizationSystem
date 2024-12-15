using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities.Settings.AlgorithmSettings;

public class AgglomerativeSettings : ClusterAlgorithmSettings
{
    private const float DefaultThreshold = 0.75f;

    [Required]
    public float Threshold { get; set; } 

    public AgglomerativeSettings()
    {
        Threshold = DefaultThreshold;
    }
}
