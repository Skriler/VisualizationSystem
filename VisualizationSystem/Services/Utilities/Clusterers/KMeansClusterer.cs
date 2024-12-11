using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class KMeansClusterer : IClusterize
{
    private readonly DataNormalizer dataNormalizer;

    private readonly Random random = new();
    private readonly int k;
    private readonly int maxIterations;

    private List<KMeansCluster> kMeansClusters;

    public KMeansClusterer(DataNormalizer dataNormalizer, int k = 5, int maxIterations = 100)
    {
        this.dataNormalizer = dataNormalizer;

        this.k = k;
        this.maxIterations = maxIterations;
    }

    public List<Cluster> Cluster(List<NodeObject> nodes, float minSimilarityThreshold)
    {
        var normalizedNodes = dataNormalizer.GetNormalizedData(nodes);
        var parametersCount = normalizedNodes.FirstOrDefault()?.NormalizedParameters.Length ?? 0;

        if (normalizedNodes.Count < k)
            throw new InvalidOperationException("Nodes amount is less than the number of clusters");

        kMeansClusters = new List<KMeansCluster>(k);
        InitializeClusters(normalizedNodes);

        for (int iteration = 0; iteration < maxIterations; ++iteration)
        {
            var assignmentsChanged = false;

            foreach (var kMeansCluster in kMeansClusters)
                kMeansCluster.Cluster.Nodes.Clear();

            for (int i = 0; i < normalizedNodes.Count; ++i)
            {
                var clusterIndex = GetNearestClusterIndex(normalizedNodes[i]);

                if (kMeansClusters[clusterIndex].Cluster.Nodes.Contains(nodes[i])) 
                    continue;

                assignmentsChanged = true;
                kMeansClusters[clusterIndex].Cluster.AddNode(nodes[i]);
            }

            if (!assignmentsChanged)
                break;

            RecalculateCentroids(normalizedNodes, parametersCount);
        }

        return kMeansClusters.Select(c => c.Cluster).ToList();
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

    private void InitializeClusters(List<NormalizedNodeData> nodes)
    {
        var selectedIndices = new HashSet<int>();

        for (int i = 0; i < k; ++i)
        {
            int randomIndex;

            do
            {
                randomIndex = random.Next(nodes.Count);
            } while (!selectedIndices.Add(randomIndex));

            kMeansClusters[i].InitializeCentroid(nodes[randomIndex].NormalizedParameters);
        }
    }

    private int GetNearestClusterIndex(NormalizedNodeData node)
    {
        var clusterIndex = 0;
        var minDistance = double.MaxValue;
        
        for (int i = 0; i < kMeansClusters.Count; ++i)
        {
            var distance = GetEuclideanDistance(node, kMeansClusters[i].Centroid);

            if (distance >= minDistance)
                continue;

            minDistance = distance;
            clusterIndex = i;
        }

        return clusterIndex;
    }

    private void RecalculateCentroids(List<NormalizedNodeData> data, int cols)
    {
        foreach (var kMeansCluster in kMeansClusters)
        {
            var clusterData = GetClusterData(data, kMeansCluster.Cluster, cols);
            kMeansCluster.RecalculateCentroid(clusterData);
        }
    }

    private List<double[]> GetClusterData(List<NormalizedNodeData> data, Cluster cluster, int cols)
    {
        var clusterData = new List<double[]>();

        foreach (var node in cluster.Nodes)
        {
            var nodeData = data.FirstOrDefault(nd => nd.Node.Name == node.Name);

            if (nodeData == null)
                continue;

            clusterData.Add(nodeData.NormalizedParameters);
        }

        return clusterData;
    }

    private double GetEuclideanDistance(NormalizedNodeData node, double[] centroid)
    {
        double distance = 0;
        var data = node.NormalizedParameters;

        for (int i = 0; i < centroid.Length; ++i)
        {
            var diff = data[i] - centroid[i];
            distance += diff * diff;
        }

        return Math.Sqrt(distance);
    }
}
