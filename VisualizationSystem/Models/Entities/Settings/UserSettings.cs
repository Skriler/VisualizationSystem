using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Models.Entities.Settings;

public class UserSettings
{
    private const float DefaultMinSimilarityPercentage = 55f;
    private const float DefaultDeviationPercent = 10f;

    [Key]
    public int Id { get; set; }

    [Required]
    public float MinSimilarityPercentage { get; set; }

    [Required]
    public float DeviationPercent { get; set; }

    [Required]
    public bool UseClustering { get; set; }

    [NotMapped]
    public bool UseNormalGraph { get => !UseClustering; }

    [Required]
    public bool UseClusteredGraph { get; set; }

    [Required]
    public int NodeTableId { get; set; }
    public NodeTable NodeTable { get; set; } = default!;

    public int AlgorithmSettingsId { get; set; }
    public ClusterAlgorithmSettings AlgorithmSettings { get; set; } = default!;

    public List<ParameterState> ParameterStates { get; set; } = new();

    public UserSettings()
    {
        ResetCoreValues();
    }

    public void ResetToDefaults()
    {
        ResetCoreValues();
        ResetParameterStates();
        AlgorithmSettings.ResetToDefaults();
    }

    public void ResetCoreValues()
    {
        MinSimilarityPercentage = DefaultMinSimilarityPercentage;
        DeviationPercent = DefaultDeviationPercent;
        UseClustering = false;
        UseClusteredGraph = false;
    }

    public void ResetParameterStates()
    {
        ParameterStates.ForEach(p => p.ResetToDefaults());
    }
}
