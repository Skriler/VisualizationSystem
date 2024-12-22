using VisualizationSystem.Models.Entities.Nodes.Normalized;

namespace VisualizationSystem.Models.Storages.Clusters;

public class KMeansCluster : Cluster
{
    public List<double> Centroid { get; set; } = default!;

    public KMeansCluster(List<NormalizedNodeParameter> nodeParameters)
    {
        Centroid = nodeParameters.Select(np => np.Value).ToList();
    }

    public void RecalculateCentroid(List<NormalizedNode> normalizedNodes)
    {
        if (normalizedNodes.Any(nn => nn.NormalizedParameters.Count != Centroid.Count))
            throw new InvalidOperationException("Node parameter count does not match centroid's");

        Centroid = Enumerable.Range(0, Centroid.Count)
            .Select(col => normalizedNodes.Average(nn => nn.NormalizedParameters[col].Value))
            .ToList();
    }
}
