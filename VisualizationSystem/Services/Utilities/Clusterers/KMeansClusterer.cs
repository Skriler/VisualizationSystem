using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class KMeansClusterer : IClusterize
{
    private readonly DataNormalizer dataNormalizer;

    private readonly Random random = new();
    private readonly int k;
    private readonly int maxIterations;

    public KMeansClusterer(DataNormalizer dataNormalizer, int k = 20, int maxIterations = 100)
    {
        this.dataNormalizer = dataNormalizer;

        this.k = k;
        this.maxIterations = maxIterations;
    }

    public List<Cluster> Cluster(List<NodeObject> nodes, float minSimilarityThreshold)
    {
        var clusters = new List<Cluster>();

        dataNormalizer.NormalizeNodeParameters(nodes);
        var matrixManager = dataNormalizer.matrixManager;

        Console.WriteLine(matrixManager);

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

    private List<float[]> InitializeCentroids(List<float[]> features)
    {
        var centroids = new List<float[]>();
        var chosenIndices = new HashSet<int>();

        while (centroids.Count < k)
        {
            var index = random.Next(features.Count);

            if (chosenIndices.Contains(index))
                continue;

            centroids.Add((float[])features[index].Clone());
            chosenIndices.Add(index);
        }

        return centroids;
    }

    private List<(int NodeIndex, int ClusterId)> AssignNodesToClusters(List<float[]> features, List<float[]> centroids)
    {
        var assignments = new List<(int NodeIndex, int ClusterId)>();

        for (int i = 0; i < features.Count; i++)
        {
            var clusterId = GetNearestCentroid(features[i], centroids);
            assignments.Add((i, clusterId));
        }

        return assignments;
    }

    private int GetNearestCentroid(float[] feature, List<float[]> centroids)
    {
        var minDistance = float.MaxValue;
        var clusterId = 0;

        for (int i = 0; i < centroids.Count; i++)
        {
            var distance = GetEuclideanDistance(feature, centroids[i]);

            if (distance >= minDistance)
                continue;

            minDistance = distance;
            clusterId = i;
        }

        return clusterId;
    }

    private List<float[]> RecalculateCentroids(List<(int NodeIndex, int ClusterId)> assignments, List<float[]> features)
    {
        var centroids = new List<float[]>(k);
        var clusterCounts = new int[k];

        for (int i = 0; i < k; i++)
        {
            centroids.Add(new float[features[0].Length]);
        }

        foreach (var assignment in assignments)
        {
            var feature = features[assignment.NodeIndex];
            var clusterId = assignment.ClusterId;

            for (int j = 0; j < feature.Length; j++)
            {
                centroids[clusterId][j] += feature[j];
            }

            clusterCounts[clusterId]++;
        }

        for (int i = 0; i < k; i++)
        {
            if (clusterCounts[i] == 0)
                continue;

            for (int j = 0; j < centroids[i].Length; j++)
            {
                centroids[i][j] /= clusterCounts[i];
            }
        }

        return centroids;
    }

    private bool AreCentroidsEqual(List<float[]> firstCentroids, List<float[]> secondCentroids)
    {
        for (int i = 0; i < firstCentroids.Count; i++)
        {
            if (firstCentroids[i].SequenceEqual(secondCentroids[i]))
                continue;

            return false;
        }

        return true;
    }

    private float GetEuclideanDistance(float[] point1, float[] point2)
    {
        float distance = 0;

        for (int i = 0; i < point1.Length; i++)
        {
            var diff = point1[i] - point2[i];

            distance += diff * diff;
        }

        return (float)Math.Sqrt(distance);
    }
}
