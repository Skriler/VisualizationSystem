using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages.Clusters;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public interface IClusterize
{
    List<Cluster> Cluster(List<NodeObject> nodes, float minSimilarityThreshold);
}
