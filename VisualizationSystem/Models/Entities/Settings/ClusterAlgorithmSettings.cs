using System.ComponentModel.DataAnnotations;
using VisualizationSystem.Models.Domain.Settings;
using VisualizationSystem.Services.Utilities.Clusterers;

namespace VisualizationSystem.Models.Entities.Settings;

public class ClusterAlgorithmSettings
{
    private const ClusterAlgorithm DefaultSelectedAlgorithm = ClusterAlgorithm.KMeans;
    private const double DefaultThreshold = 0.4f;
    private const int DefaultNumberOfClusters = 7;
    private const int DefaultMaxIterations = 500;
    private const double DefaultEpsilon = 0.15;
    private const int DefaultMinPoints = 2;

    [Key]
    public int Id { get; set; }

    [Required]
    public ClusterAlgorithm SelectedAlgorithm { get; set; }

    [Required]
    public double Threshold { get; set; }

    [Required]
    public int NumberOfClusters { get; set; }

    [Required]
    public int MaxIterations { get; set; }

    [Required]
    public double Epsilon { get; set; }

    [Required]
    public int MinPoints { get; set; }

    [Required]
    public int UserSettingsId { get; set; }
    public UserSettings UserSettings { get; set; } = default!;

    public ClusterAlgorithmSettings()
    {
        ResetToDefaults();
    }

    public void ResetToDefaults()
    {
        SelectedAlgorithm = DefaultSelectedAlgorithm;
        Threshold = DefaultThreshold;
        NumberOfClusters = DefaultNumberOfClusters;
        MaxIterations = DefaultMaxIterations;
        Epsilon = DefaultEpsilon;
        MinPoints = DefaultMinPoints;
    }

    public void SetData(ClusterAlgorithmSettingsData settingsData)
    {
        SelectedAlgorithm = settingsData.SelectedAlgorithm;
        Threshold = settingsData.Threshold;
        NumberOfClusters = settingsData.NumberOfClusters;
        MaxIterations = settingsData.MaxIterations;
        Epsilon = settingsData.Epsilon;
        MinPoints = settingsData.MinPoints;
    }
}
