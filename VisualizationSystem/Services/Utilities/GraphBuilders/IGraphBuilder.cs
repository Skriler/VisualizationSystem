using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.Models.Storages.Clusters;

namespace VisualizationSystem.Services.Utilities.GraphBuilders;

public interface IGraphBuilder<TGraph>
{
    public Dictionary<string, NodeSimilarityResult> NodeDataMap { get; }
    public UserSettings Settings { get; protected set; }

    TGraph Build(string name, List<NodeSimilarityResult> similarityResults);
    TGraph Build(string name, List<Cluster> clusters);

    void UpdateSettings(UserSettings settings) => Settings = settings;
}
