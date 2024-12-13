using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.Models.Storages.Clusters;

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
        var normalizedNodes = dataNormalizer.GetNormalizedNodes(nodes);
        var parametersCount = normalizedNodes.FirstOrDefault()?.NormalizedParameters.Count ?? 0;

        if (normalizedNodes.Count < k)
            throw new InvalidOperationException("Nodes amount is less than the number of clusters");

        kMeansClusters = new List<KMeansCluster>(k);
        InitializeClusters(normalizedNodes, parametersCount);

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

            RecalculateClusters(normalizedNodes, parametersCount);
        }

        return kMeansClusters.Select(c => c.Cluster).ToList();
    }

    private void InitializeClusters(List<NormalizedNode> nodes, int parametersCount)
    {
        var selectedIndices = new HashSet<int>();

        for (int i = 0; i < k; ++i)
        {
            var cluster = new KMeansCluster(parametersCount);

            int randomIndex;
            do
            {
                randomIndex = random.Next(nodes.Count);
            } while (!selectedIndices.Add(randomIndex));

            cluster.InitializeCentroid(nodes[randomIndex].NormalizedParameters);
            kMeansClusters.Add(cluster);
        }
    }

    private int GetNearestClusterIndex(NormalizedNode node)
    {
        var clusterIndex = 0;
        var minDistance = double.MaxValue;
        
        for (int i = 0; i < kMeansClusters.Count; ++i)
        {
            var distance = GetEuclideanDistance(node.NormalizedParameters, kMeansClusters[i].Centroid);

            if (distance >= minDistance)
                continue;

            minDistance = distance;
            clusterIndex = i;
        }

        return clusterIndex;
    }

    private void RecalculateClusters(List<NormalizedNode> data, int cols)
    {
        foreach (var kMeansCluster in kMeansClusters)
        {
            var clusterData = GetClusterData(data, kMeansCluster.Cluster, cols);
            kMeansCluster.RecalculateCentroid(clusterData);
        }
    }

    private List<List<double>> GetClusterData(List<NormalizedNode> data, Cluster cluster, int cols)
    {
        var clusterData = new List<List<double>>();

        foreach (var node in cluster.Nodes)
        {
            var nodeData = data.FirstOrDefault(nd => nd.Node.Name == node.Name);

            if (nodeData == null)
                continue;

            clusterData.Add(nodeData.NormalizedParameters);
        }

        return clusterData;
    }

    private double GetEuclideanDistance(List<double> data, double[] centroid)
    {
        if (data.Count != centroid.Length)
            throw new InvalidOperationException("Node data and centroid must be the same length");

        double distance = 0;

        for (int i = 0; i < centroid.Length; ++i)
        {
            var diff = data[i] - centroid[i];
            distance += diff * diff;
        }

        return Math.Sqrt(distance);
    }
}
