using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Storages.Clusters;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public interface IClusterize
{
    public ClusterAlgorithmSettings AlgorithmSettings { get; protected set; }

    List<Cluster> Cluster(List<NodeObject> nodes);

    void UpdateSettings(ClusterAlgorithmSettings algorithmSettings) => AlgorithmSettings = algorithmSettings;
}
