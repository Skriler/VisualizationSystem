namespace VisualizationSystem.Models.Storages;

public class KMeansCluster
{
    public Cluster Cluster { get; set; }
    public double[] Centroid { get; set; }

    public KMeansCluster(int parameterCount)
    {
        Cluster = new Cluster();
        Centroid = new double[parameterCount];
    }

    public void InitializeCentroid(double[] nodeData)
    {
        if (nodeData.Length != Centroid.Length)
            throw new InvalidOperationException("Размерность данных не совпадает с размерностью центроида");

        nodeData.CopyTo(Centroid, 0);
    }

    public void RecalculateCentroid(List<double[]> clusterData)
    {
        for (int col = 0; col < Centroid.Length; ++col)
        {
            Centroid[col] = clusterData.Average(dataRow => dataRow[col]);
        }
    }
}
