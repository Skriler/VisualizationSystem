using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Services.Utilities.Graph.Builders;

public interface IGraphBuilder<TGraph>
{
    TGraph Build(string name, List<NodeSimilarityResult> similarityResults);

    TGraph Build(string name, List<NodeObject> nodes, List<Cluster> clusters);

    TGraph Build(string name, List<NodeSimilarityResult> similarityResults, List<Cluster> clusters);
}
