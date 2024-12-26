using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Nodes.Normalized;
using VisualizationSystem.Services.Utilities.Normalizers;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class KMeansClusterer : BaseClusterer
{
    private readonly Random random = new();
    private List<KMeansCluster> kMeansClusters;

    public KMeansClusterer(DataNormalizer dataNormalizer, MetricDistanceCalculator distanceCalculator)
        : base(dataNormalizer, distanceCalculator)
    { }

    public override async Task<List<Cluster>> ClusterAsync(NodeTable nodeTable)
    {
        var normalizedNodes = await dataNormalizer.GeNormalizedNodesAsync(nodeTable);
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

                if (kMeansClusters[clusterIndex].Nodes.Contains(nodeTable.NodeObjects[i])) 
                    continue;

                assignmentsChanged = true;
                kMeansClusters[clusterIndex].AddNode(nodeTable.NodeObjects[i]);
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
            int randomIndex;
            do
            {
                randomIndex = random.Next(nodes.Count);
            } while (!selectedIndices.Add(randomIndex));

            var cluster = new KMeansCluster(nodes[randomIndex].NormalizedParameters);
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
            var clusterData = GetClusterNodes(data, kMeansCluster);
            kMeansCluster.RecalculateCentroid(clusterData);
        }
    }

    private List<NormalizedNode> GetClusterNodes(List<NormalizedNode> data, KMeansCluster cluster)
    {
        var clusterNodes = new List<NormalizedNode>();

        foreach (var node in cluster.Nodes)
        {
            var normalizedNode = data.FirstOrDefault(nd => nd.NodeObject.Name == node.Name);

            if (normalizedNode == null)
                continue;

            clusterNodes.Add(normalizedNode);
        }

        return clusterNodes;
    }

    private double GetEuclideanDistance(List<NormalizedNodeParameter> data, List<double> centroid)
    {
        if (data.Count != centroid.Count)
            throw new InvalidOperationException("Node data and centroid must be the same length");

        double distance = 0;

        for (int i = 0; i < centroid.Count; ++i)
        {
            var diff = data[i].Value - centroid[i];
            distance += diff * diff;
        }

        return Math.Sqrt(distance);
    }
}
