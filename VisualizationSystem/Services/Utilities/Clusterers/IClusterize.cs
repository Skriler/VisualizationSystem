using VisualizationSystem.Models.Storages;
using VisualizationSystem.Models.Storages.Clusters;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public interface IClusterize
{
    List<Cluster> Cluster(List<NodeSimilarityResult> similarityResults, float minSimilarityThreshold);
}
