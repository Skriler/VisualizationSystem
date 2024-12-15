using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Storages.Clusters;
using VisualizationSystem.Models.Storages.Results;

namespace VisualizationSystem.Services.Utilities.GraphBuilders;

public interface IGraphBuilder<TGraph>
{
    public Dictionary<string, NodeSimilarityResult> NodeDataMap { get; }
    public UserSettings Settings { get; protected set; }

    TGraph Build(string name, List<NodeSimilarityResult> similarityResults);
    TGraph Build(string name, List<NodeObject> nodes, List<Cluster> clusters);

    void UpdateSettings(UserSettings settings) => Settings = settings;
}
