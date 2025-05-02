using VisualizationSystem.Models.Domain.Nodes;

namespace VisualizationSystem.Models.Domain.Clusters;

public class KMeansCluster : Cluster
{
    public CalculationNode Centroid { get; set; }

    public KMeansCluster(CalculationNode node)
    {
        Centroid = new CalculationNode(node);
    }
}
