using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.Utilities.Clusterers;

namespace VisualizationSystem.Models.Domain.Settings;

public class ClusterAlgorithmSettingsData
{
    public ClusterAlgorithm SelectedAlgorithm { get; set; }

    public float Threshold { get; set; }

    public int NumberOfClusters { get; set; }

    public int MaxIterations { get; set; }

    public float Epsilon { get; set; }

    public int MinPoints { get; set; }

    public ClusterAlgorithmSettingsData(ClusterAlgorithmSettings settings)
    {
        SetData(settings);
    }

    public void SetData(ClusterAlgorithmSettings settings)
    {
        SelectedAlgorithm = settings.SelectedAlgorithm;
        Threshold = settings.Threshold;
        NumberOfClusters = settings.NumberOfClusters;
        MaxIterations = settings.MaxIterations;
        Epsilon = settings.Epsilon;
        MinPoints = settings.MinPoints;
    }
}
