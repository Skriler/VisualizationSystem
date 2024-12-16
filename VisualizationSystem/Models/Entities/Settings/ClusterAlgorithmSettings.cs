using System.ComponentModel.DataAnnotations;
using VisualizationSystem.Services.Utilities.Clusterers;

namespace VisualizationSystem.Models.Entities.Settings;

public class ClusterAlgorithmSettings
{
    private const ClusterAlgorithm DefaultSelectedAlgorithm = ClusterAlgorithm.Agglomerative;
    private const float DefaultThreshold = 0.75f;
    private const int DefaultNumberOfClusters = 5;
    private const int DefaultMaxIterations = 100;
    private const float DefaultEpsilon = 4;
    private const int DefaultMinPoints = 4;

    [Key]
    public int Id { get; set; }

    [Required]
    public ClusterAlgorithm SelectedAlgorithm { get; set; }

    [Required]
    public float Threshold { get; set; }

    [Required]
    public int NumberOfClusters { get; set; }

    [Required]
    public int MaxIterations { get; set; }

    [Required]
    public float Epsilon { get; set; }

    [Required]
    public int MinPoints { get; set; }

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
}
