using VisualizationSystem.Models.Entities.Nodes.Normalized;

namespace VisualizationSystem.Models.Storages.Clusters;

public class KMeansCluster : Cluster
{
    public double[] Centroid { get; set; }

    public KMeansCluster(int parameterCount)
    {
        Centroid = new double[parameterCount];
    }

    public void InitializeCentroid(List<double> nodeData)
    {
        if (nodeData.Count != Centroid.Length)
            throw new InvalidOperationException("Размерность данных не совпадает с размерностью центроида");

        nodeData.CopyTo(Centroid, 0);
    }

    public void RecalculateCentroid(List<List<NormalizedNodeParameter>> clusterData)
    {
        for (int col = 0; col < Centroid.Length; ++col)
        {
            Centroid[col] = clusterData.Average(dataRow => dataRow[col].Value);
        }
    }
}
