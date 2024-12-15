using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Storages.Clusters;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public interface IClusterize
{
    public UserSettings Settings { get; protected set; }

    List<Cluster> Cluster(List<NodeObject> nodes);

    void UpdateSettings(UserSettings settings) => Settings = settings;
}
