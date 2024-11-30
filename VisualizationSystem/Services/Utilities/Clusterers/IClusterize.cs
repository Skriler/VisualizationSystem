using VisualizationSystem.Models.Storages;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public interface IClusterize
{
    List<Cluster> Cluster(List<NodeSimilarityResult> similarityResults, float minSimilarityThreshold);
}
