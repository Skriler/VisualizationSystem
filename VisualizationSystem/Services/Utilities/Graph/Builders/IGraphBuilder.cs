using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;

namespace VisualizationSystem.Services.Utilities.Graph.Builders;

public interface IGraphBuilder<TGraph>
{
    public UserSettings Settings { get; protected set; }

    TGraph Build(string name, List<NodeSimilarityResult> similarityResults);
    TGraph Build(string name, List<NodeObject> nodes, List<Cluster> clusters);

    void UpdateSettings(UserSettings settings) => Settings = settings;
}
