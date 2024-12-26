using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Domain.Clusters;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public abstract class BaseClusterer
{
    protected readonly DataNormalizer dataNormalizer;
    protected readonly MetricDistanceCalculator distanceCalculator;

    public ClusterAlgorithmSettings AlgorithmSettings { get; set; }

    protected BaseClusterer(DataNormalizer dataNormalizer, MetricDistanceCalculator distanceCalculator)
    {
        this.dataNormalizer = dataNormalizer;
        this.distanceCalculator = distanceCalculator;
    }

    public abstract Task<List<Cluster>> ClusterAsync(NodeTable nodeTable);

    public void UpdateSettings(ClusterAlgorithmSettings algorithmSettings) => AlgorithmSettings = algorithmSettings;
}
