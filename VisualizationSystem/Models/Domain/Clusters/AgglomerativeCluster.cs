using VisualizationSystem.Models.Domain.Nodes;

namespace VisualizationSystem.Models.Domain.Clusters;

public class AgglomerativeCluster : Cluster
{
    public bool IsMerged { get; set; }

    public AgglomerativeCluster(CalculationNode node)
    {
        IsMerged = false;
        Nodes.Add(node);
    }

    public void Merge(AgglomerativeCluster mergedCluster)
    {
        Nodes.AddRange(mergedCluster.Nodes);
        mergedCluster.IsMerged = true;
    }
}