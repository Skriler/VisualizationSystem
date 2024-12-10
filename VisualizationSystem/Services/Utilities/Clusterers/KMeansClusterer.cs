using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class KMeansClusterer : IClusterize
{
    private readonly DataNormalizer dataNormalizer;

    private readonly Random random = new();
    private readonly int k;
    private readonly int maxIterations;

    public KMeansClusterer(DataNormalizer dataNormalizer, int k = 5, int maxIterations = 100)
    {
        this.dataNormalizer = dataNormalizer;

        this.k = k;
        this.maxIterations = maxIterations;
    }

    public List<Cluster> Cluster(List<NodeObject> nodes, float minSimilarityThreshold)
    {
        var matrix = dataNormalizer.GetNormalizedMatrix(nodes);

        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);

        if (rows < k)
            throw new InvalidOperationException("Nodes amount is less than the number of clusters");

        var centroids = InitializeCentroids(matrix);
        var clusters = GetEmptyClusters();

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            var assignmentsChanged = false;

            for (int i = 0; i < rows; i++)
            {
                var nearestCluster = GetNearestClusterIndex(matrix, centroids, i);

                if (clusters[nearestCluster].Nodes.All(n => n != nodes[i]))
                {
                    assignmentsChanged = true;
                    clusters[nearestCluster].AddNode(nodes[i]);
                }
            }

            if (!assignmentsChanged)
                break;

            centroids = RecalculateCentroids(matrix, clusters, cols);
        }

        return clusters;
    }

    public List<Cluster> Cluster(List<NodeSimilarityResult> similarityResults, float minSimilarityThreshold)
    {
        var nodes = similarityResults.Select(sr => sr.Node).ToList();
        //var features = TODO get

        //var centroids = InitializeCentroids(features);

        var clusters = new List<Cluster>();

        //for (int iteration = 0; iteration < maxIterations; iteration++)
        //{
        //    var assignments = AssignNodesToClusters(features, centroids);
        //    var newCentroids = RecalculateCentroids(assignments, features);

        //    if (AreCentroidsEqual(centroids, newCentroids))
        //        break;

        //    centroids = newCentroids;
        //}

        return clusters;
    }

    private List<double[]> InitializeCentroids(double[,] data)
    {
        var rows = data.GetLength(0);
        var cols = data.GetLength(1);

        var centroids = new List<double[]>();
        var selectedIndices = new HashSet<int>();

        while (centroids.Count < k)
        {
            var randomIndex = random.Next(rows);

            if (!selectedIndices.Add(randomIndex))
                continue;

            centroids.Add(CreateCentroidFromData(data, randomIndex, cols));
        }

        return centroids;
    }

    private double[] CreateCentroidFromData(double[,] data, int rowIndex, int cols)
    {
        var centroid = new double[cols];

        for (int col = 0; col < cols; ++col)
        {
            centroid[col] = data[rowIndex, col];
        }

        return centroid;
    }

    private List<Cluster> GetEmptyClusters()
    {
        var clusters = new List<Cluster>();

        for (int i = 0; i < k; ++i)
        {
            clusters.Add(new Cluster());
        }

        return clusters;
    }

    private int GetNearestClusterIndex(double[,] matrix, List<double[]> centroids, int rowIndex)
    {
        var nearestCluster = 0;
        var minDistance = double.MaxValue;

        for (int cluster = 0; cluster < centroids.Count; ++cluster)
        {
            var distance = GetEuclideanDistance(matrix, centroids[cluster], rowIndex);

            if (distance >= minDistance)
                continue;

            minDistance = distance;
            nearestCluster = cluster;
        }

        return nearestCluster;
    }

    private List<double[]> RecalculateCentroids(double[,] matrix, List<Cluster> clusters, int cols)
    {
        var centroids = new List<double[]>();

        for (int i = 0; i < k; i++)
        {
            var cluster = clusters[i];

            if (cluster.Nodes.Count == 0)
            {
                centroids.Add(new double[cols]);
                continue;
            }

            var clusterData = GetClusterData(matrix, cluster, cols);
            var newCentroid = RecalculateCentroidFromClusterData(clusterData, cols);

            centroids.Add(newCentroid);
        }

        return centroids;
    }

    private List<double[]> GetClusterData(double[,] matrix, Cluster cluster, int cols)
    {
        var clusterData = new List<double[]>();

        foreach (var node in cluster.Nodes)
        {
            //int nodeIndex = // TODO get node Index
            var dataRow = new double[cols];

            for (int col = 0; col < cols; col++)
            {
                //dataRow[col] = matrix[nodeIndex, col];
            }

            clusterData.Add(dataRow);
        }

        return clusterData;
    }

    private double[] RecalculateCentroidFromClusterData(List<double[]> clusterData, int cols)
    {
        var centroid = new double[cols];

        foreach (var dataRow in clusterData)
        {
            for (int col = 0; col < cols; ++col)
            {
                centroid[col] += dataRow[col];
            }
        }

        for (int col = 0; col < cols; ++col)
        {
            centroid[col] /= clusterData.Count;
        }

        return centroid;
    }

    private double GetEuclideanDistance(double[,] matrix, double[] centroid, int rowIndex)
    {
        double distance = 0;

        for (int col = 0; col < centroid.Length; ++col)
        {
            var diff = matrix[rowIndex, col] - centroid[col];
            distance += diff * diff;
        }

        return Math.Sqrt(distance);
    }
}
