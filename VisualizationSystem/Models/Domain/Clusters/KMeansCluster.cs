using VisualizationSystem.Models.Domain.Nodes;

namespace VisualizationSystem.Models.Domain.Clusters;

public class KMeansCluster : Cluster
{
    public Centroid Centroid { get; set; }

    public KMeansCluster(CalculationNode node)
    {
        Centroid = new Centroid(node);
    }

    public void RecalculateCentroid()
    {
        if (Nodes.Any(n => n.Parameters.Count != Centroid.Parameters.Count))
            throw new InvalidOperationException("Node parameter count does not match centroid's");

        Centroid.Merge(Nodes);
    }
}
