using VisualizationSystem.Models.Entities.Nodes.Normalized;

namespace VisualizationSystem.Models.Domain.Clusters;

public class KMeansCluster : Cluster
{
    public List<double> Centroid { get; set; } = default!;

    public KMeansCluster(List<NormParameter> nodeParameters)
    {
        Centroid = nodeParameters.Select(np => np.Value).ToList();
    }

    public void RecalculateCentroid(List<NormNode> normalizedNodes)
    {
        if (normalizedNodes.Any(nn => nn.NormParameters.Count != Centroid.Count))
            throw new InvalidOperationException("Node parameter count does not match centroid's");

        Centroid = Enumerable.Range(0, Centroid.Count)
            .Select(col => normalizedNodes.Average(nn => nn.NormParameters[col].Value))
            .ToList();
    }
}
