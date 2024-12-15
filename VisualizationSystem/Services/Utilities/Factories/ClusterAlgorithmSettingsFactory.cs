using VisualizationSystem.Models.Entities.Settings.AlgorithmSettings;
using VisualizationSystem.Services.Utilities.Clusterers;

namespace VisualizationSystem.Services.Utilities.Factories;

public class ClusterAlgorithmSettingsFactory
{
    public ClusterAlgorithmSettings CreateAlgorithmSettings(ClusterAlgorithm algorithm)
    {
        return algorithm switch
        {
            ClusterAlgorithm.Agglomerative => new AgglomerativeSettings(),
            ClusterAlgorithm.KMeans => new KMeansSettings(),
            ClusterAlgorithm.DBSCAN => new DBSCANSettings(),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm))
        };
    }
}
