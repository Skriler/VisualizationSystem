namespace VisualizationSystem.Models.Storages.Clusters;

public class KMeansCluster
{
    public Cluster Cluster { get; set; }
    public double[] Centroid { get; set; }

    public KMeansCluster(int parameterCount)
    {
        Cluster = new Cluster();
        Centroid = new double[parameterCount];
    }

    public void InitializeCentroid(List<double> nodeData)
    {
        if (nodeData.Count != Centroid.Length)
            throw new InvalidOperationException("Размерность данных не совпадает с размерностью центроида");

        nodeData.CopyTo(Centroid, 0);
    }

    public void RecalculateCentroid(List<List<double>> clusterData)
    {
        for (int col = 0; col < Centroid.Length; ++col)
        {
            Centroid[col] = clusterData.Average(dataRow => dataRow[col]);
        }
    }
}
