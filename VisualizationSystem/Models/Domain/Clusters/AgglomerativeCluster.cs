using VisualizationSystem.Models.Domain.Nodes;

namespace VisualizationSystem.Models.Domain.Clusters;

public class AgglomerativeCluster : Cluster
{
    public bool IsMerged { get; set; }

    public List<CalculationNode> CalculationNodes { get; private set; }

    public AgglomerativeCluster(CalculationNode node)
    {
        IsMerged = false;
        CalculationNodes = new List<CalculationNode>();
        CalculationNodes.Add(node);

        Nodes.Add(node.Entity);
    }

    public void Merge(AgglomerativeCluster mergedCluster)
    {
        Nodes.AddRange(mergedCluster.Nodes);
        CalculationNodes.AddRange(mergedCluster.CalculationNodes);
        mergedCluster.IsMerged = true;
    }
}