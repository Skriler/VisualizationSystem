using Microsoft.Extensions.DependencyInjection;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class ClustererFactory
{
    private readonly IServiceProvider serviceProvider;

    public ClustererFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public IClusterize CreateClusterer(ClusterAlgorithm algorithm)
    {
        return algorithm switch
        {
            ClusterAlgorithm.Agglomerative => serviceProvider.GetRequiredService<AgglomerativeClusterer>(),
            ClusterAlgorithm.KMeans => serviceProvider.GetRequiredService<KMeansClusterer>(),
            ClusterAlgorithm.DBSCAN => serviceProvider.GetRequiredService<DBSCANClusterer>(),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm))
        };
    }
}