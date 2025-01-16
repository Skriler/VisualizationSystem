using Microsoft.Extensions.DependencyInjection;
using VisualizationSystem.Services.Utilities.Clusterers;
using VisualizationSystem.Services.Utilities.DistanceCalculators;

namespace VisualizationSystem.Services.Utilities.Factories;

public class ClustererFactory
{
    private readonly IServiceProvider serviceProvider;

    public ClustererFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public BaseClusterer Create(ClusterAlgorithm algorithm)
    {
        return algorithm switch
        {
            ClusterAlgorithm.KMeans => serviceProvider.GetRequiredService<KMeansClusterer>(),
            ClusterAlgorithm.DBSCAN => serviceProvider.GetRequiredService<DBSCANClusterer>(),
            ClusterAlgorithm.HierarchicalAgglomerative => serviceProvider.GetRequiredService<AgglomerativeClusterer>(),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm))
        };
    }
}