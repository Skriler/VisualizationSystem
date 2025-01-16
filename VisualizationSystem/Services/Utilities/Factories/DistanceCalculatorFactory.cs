using Microsoft.Extensions.DependencyInjection;
using VisualizationSystem.Models.Configs.DistanceMetrics;
using VisualizationSystem.Services.Utilities.Clusterers;
using VisualizationSystem.Services.Utilities.DistanceCalculators;
using VisualizationSystem.Services.Utilities.DistanceCalculators.CategoricalMetrics;
using VisualizationSystem.Services.Utilities.DistanceCalculators.NumericMetrics;

namespace VisualizationSystem.Services.Utilities.Factories;

public class DistanceCalculatorFactory
{
    private readonly IServiceProvider serviceProvider;

    public DistanceCalculatorFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public IDistanceCalculator Create(ClusterAlgorithm algorithm)
    {
        var distanceCalculator = serviceProvider.GetRequiredService<IDistanceCalculator>();

        var distanceMetricsConfig = GetDistanceCalculator(algorithm);
        distanceCalculator.Initialize(distanceMetricsConfig);

        return distanceCalculator;
    }

    private DistanceMetricsConfig GetDistanceCalculator(ClusterAlgorithm algorithm)
    {
        return algorithm switch
        {
            ClusterAlgorithm.KMeans => new DistanceMetricsConfig(
                serviceProvider.GetRequiredService<EuclideanDistanceMetric>(),
                serviceProvider.GetRequiredService<HammingDistanceMetric>()
            ),
            ClusterAlgorithm.DBSCAN => new DistanceMetricsConfig(
                serviceProvider.GetRequiredService<EuclideanDistanceMetric>(),
                serviceProvider.GetRequiredService<HammingDistanceMetric>()
            ),
            ClusterAlgorithm.HierarchicalAgglomerative => new DistanceMetricsConfig(
                serviceProvider.GetRequiredService<EuclideanDistanceMetric>(),
                serviceProvider.GetRequiredService<HammingDistanceMetric>()
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm))
        };
    }
}
