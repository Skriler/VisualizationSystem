using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Models.Domain.Clusters;

public class KMeansCluster : Cluster
{
    public List<double> Centroid { get; set; } = default!;

    public KMeansCluster(List<NormalizedParameter> nodeParameters)
    {
        Centroid = nodeParameters.Select(np => np.Value).ToList();
    }

    public void RecalculateCentroid(List<NodeObject> nodes)
    {
        if (nodes.Any(n => n.NormalizedParameters.Count != Centroid.Count))
            throw new InvalidOperationException("Node parameter count does not match centroid's");

        Centroid = Enumerable.Range(0, Centroid.Count)
            .Select(col => nodes.Average(n => n.NormalizedParameters[col].Value))
            .ToList();
    }
}
