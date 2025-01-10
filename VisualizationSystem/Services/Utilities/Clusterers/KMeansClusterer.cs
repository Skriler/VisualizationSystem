using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.DistanceCalculators;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Services.Utilities.Settings;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class KMeansClusterer : BaseClusterer
{
    private readonly Random random = new();
    private List<KMeansCluster> kMeansClusters;

    public KMeansClusterer(
        DataNormalizer dataNormalizer,
        MetricDistanceCalculator distanceCalculator,
        ISettingsSubject settingsSubject
        )
        : base(dataNormalizer, distanceCalculator, settingsSubject)
    { }

    public override async Task<List<Cluster>> ClusterAsync(NodeTable nodeTable)
    {
        var nodes = await dataNormalizer.GetNormalizedNodesAsync(nodeTable);
        var parametersCount = nodes.FirstOrDefault()?.NormalizedParameters.Count ?? 0;

        if (nodes.Count < settings.AlgorithmSettings.NumberOfClusters)
            throw new InvalidOperationException("Nodes amount is less than the number of clusters");

        kMeansClusters = new List<KMeansCluster>(settings.AlgorithmSettings.NumberOfClusters);
        InitializeClusters(nodes, parametersCount);

        for (int iteration = 0; iteration < settings.AlgorithmSettings.MaxIterations; ++iteration)
        {
            var assignmentsChanged = false;

            foreach (var kMeansCluster in kMeansClusters)
                kMeansCluster.Nodes.Clear();

            for (int i = 0; i < nodes.Count; ++i)
            {
                var clusterIndex = GetNearestClusterIndex(nodes[i]);

                if (kMeansClusters[clusterIndex].Nodes.Contains(nodeTable.NodeObjects[i])) 
                    continue;

                assignmentsChanged = true;
                kMeansClusters[clusterIndex].AddNode(nodeTable.NodeObjects[i]);
            }

            if (!assignmentsChanged)
                break;

            RecalculateClusters(nodes);
        }

        return kMeansClusters.Cast<Cluster>().ToList();
    }

    private void InitializeClusters(List<NodeObject> nodes, int parametersCount)
    {
        var selectedIndices = new HashSet<int>();

        for (int i = 0; i < settings.AlgorithmSettings.NumberOfClusters; ++i)
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

    private int GetNearestClusterIndex(NodeObject node)
    {
        var clusterIndex = 0;
        var minDistance = double.MaxValue;
        
        for (int i = 0; i < kMeansClusters.Count; ++i)
        {
            //var distance = distanceCalculator.CalculateEuclidean(node.NormalizedParameters, kMeansClusters[i].Centroid);
            var distance = 0;

            if (distance >= minDistance)
                continue;

            minDistance = distance;
            clusterIndex = i;
        }

        return clusterIndex;
    }

    private void RecalculateClusters(List<NodeObject> data)
    {
        foreach (var kMeansCluster in kMeansClusters)
        {
            var clusterData = GetClusterNodes(data, kMeansCluster);
            kMeansCluster.RecalculateCentroid(clusterData);
        }
    }

    private List<NodeObject> GetClusterNodes(List<NodeObject> data, KMeansCluster cluster)
    {
        var clusterNodes = new List<NodeObject>();

        foreach (var node in cluster.Nodes)
        {
            var normalizedNode = data.FirstOrDefault(n => n.Name == node.Name);

            if (normalizedNode == null)
                continue;

            clusterNodes.Add(normalizedNode);
        }

        return clusterNodes;
    }
}
