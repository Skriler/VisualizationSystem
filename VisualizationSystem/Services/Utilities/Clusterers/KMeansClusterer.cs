using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Storages.Clusters;
using VisualizationSystem.Models.Storages.Nodes;
using VisualizationSystem.Services.Utilities.Normalizers;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class KMeansClusterer : IClusterize
{
    private readonly DataNormalizer dataNormalizer;
    private readonly Random random;
    private List<KMeansCluster> kMeansClusters;

    public ClusterAlgorithmSettings AlgorithmSettings { get; set; } = new();

    public KMeansClusterer(DataNormalizer dataNormalizer)
    {
        this.dataNormalizer = dataNormalizer;

        random = new Random();
    }

    public List<Cluster> Cluster(List<NodeObject> nodes)
    {
        var normalizedNodes = dataNormalizer.GetNormalizedNodes(nodes);
        var parametersCount = normalizedNodes.FirstOrDefault()?.NormalizedParameters.Count ?? 0;

        if (normalizedNodes.Count < AlgorithmSettings.NumberOfClusters)
            throw new InvalidOperationException("Nodes amount is less than the number of clusters");

        kMeansClusters = new List<KMeansCluster>(AlgorithmSettings.NumberOfClusters);
        InitializeClusters(normalizedNodes, parametersCount);

        for (int iteration = 0; iteration < AlgorithmSettings.MaxIterations; ++iteration)
        {
            var assignmentsChanged = false;

            foreach (var kMeansCluster in kMeansClusters)
                kMeansCluster.Nodes.Clear();

            for (int i = 0; i < normalizedNodes.Count; ++i)
            {
                var clusterIndex = GetNearestClusterIndex(normalizedNodes[i]);

                if (kMeansClusters[clusterIndex].Nodes.Contains(nodes[i])) 
                    continue;

                assignmentsChanged = true;
                kMeansClusters[clusterIndex].AddNode(nodes[i]);
            }

            if (!assignmentsChanged)
                break;

            RecalculateClusters(normalizedNodes);
        }

        return kMeansClusters.Cast<Cluster>().ToList();
    }

    private void InitializeClusters(List<NormalizedNode> nodes, int parametersCount)
    {
        var selectedIndices = new HashSet<int>();

        for (int i = 0; i < AlgorithmSettings.NumberOfClusters; ++i)
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

    private void RecalculateClusters(List<NormalizedNode> data)
    {
        foreach (var kMeansCluster in kMeansClusters)
        {
            var clusterData = GetClusterData(data, kMeansCluster);
            kMeansCluster.RecalculateCentroid(clusterData);
        }
    }

    private List<List<double>> GetClusterData(List<NormalizedNode> data, KMeansCluster cluster)
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
