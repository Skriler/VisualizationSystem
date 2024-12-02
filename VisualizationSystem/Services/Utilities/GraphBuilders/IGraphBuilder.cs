using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;

namespace VisualizationSystem.Services.Utilities.GraphBuilders;

public interface IGraphBuilder<TGraph>
{
    public Dictionary<string, NodeSimilarityResult> NodeDataMap { get; }
    public UserSettings Settings { get; protected set; }

    TGraph Build(string name, List<NodeSimilarityResult> similarityResults, List<Cluster> clusters);

    void UpdateSettings(UserSettings settings) => Settings = settings;
}
