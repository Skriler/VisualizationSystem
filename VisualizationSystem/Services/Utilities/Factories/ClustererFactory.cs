﻿using Microsoft.Extensions.DependencyInjection;
using VisualizationSystem.Services.Utilities.Clusterers;

namespace VisualizationSystem.Services.Utilities.Factories;

public class ClustererFactory
{
    private readonly IServiceProvider serviceProvider;

    public ClustererFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public BaseClusterer CreateClusterer(ClusterAlgorithm algorithm)
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