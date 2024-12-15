using System.ComponentModel.DataAnnotations;

namespace VisualizationSystem.Models.Entities.Settings.AlgorithmSettings;

public class KMeansSettings : ClusterAlgorithmSettings
{
    private const int DefaultNumberOfClusters = 5;
    private const int DefaultMaxIterations = 100;

    [Required] 
    public int NumberOfClusters { get; set; }

    [Required] 
    public int MaxIterations { get; set; }

    public KMeansSettings()
    {
        NumberOfClusters = DefaultNumberOfClusters;
        MaxIterations = DefaultMaxIterations;
    }
}
