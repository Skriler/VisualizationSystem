using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.Clusterers.Helpers;
using VisualizationSystem.Services.Utilities.Factories;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Services.Utilities.Settings;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class KMeansClusterer : BaseClusterer
{
    protected override ClusterAlgorithm Algorithm => ClusterAlgorithm.KMeans;

    private readonly Random random;
    private readonly CentroidCalculator centroidCalculator;

    private List<KMeansCluster> clusters = default!;

    /// <summary>
    /// A dictionary that maps each object to the index of the cluster it is assigned to.
    /// Used to track the current cluster assignment of each object, enabling the detection
    /// of changes in object assignments during the clustering process.
    /// </summary>
    private Dictionary<CalculationNode, int> objectClusterMap = new();

    public KMeansClusterer(
        DataNormalizer dataNormalizer,
        DistanceCalculatorFactory distanceCalculatorFactory,
        ISettingsSubject settingsSubject,
        CentroidCalculator centroidCalculator
        )
        : base(dataNormalizer, distanceCalculatorFactory, settingsSubject)
    {
        random = new();
        this.centroidCalculator = centroidCalculator;
    }

    public override async Task<List<Cluster>> ClusterAsync(NodeTable nodeTable)
    {
        var nodes = await dataNormalizer.GetCalculationNodesAsync(nodeTable, settings.ParameterStates);

        if (nodes.Count < settings.AlgorithmSettings.NumberOfClusters)
            throw new InvalidOperationException("Nodes amount is less than the number of clusters");

        clusters = new List<KMeansCluster>(settings.AlgorithmSettings.NumberOfClusters);
        objectClusterMap = new Dictionary<CalculationNode, int>(nodes.Count);

        InitializeClusters(nodes);
        PerformClustering(nodes);

        return clusters
            .Cast<Cluster>()
            .ToList();
    }

    /// <summary>
    /// Initializes the clusters by selecting random objects as the initial centroids.
    /// </summary>
    private void InitializeClusters(List<CalculationNode> nodes)
    {
        var selectedIndices = new HashSet<int>();

        var randomIndices = Enumerable.Range(0, nodes.Count)
            .OrderBy(_ => random.Next())
            .Take(settings.AlgorithmSettings.NumberOfClusters)
            .ToList();

        foreach (var index in randomIndices)
        {
            var cluster = new KMeansCluster(nodes[index]);
            clusters.Add(cluster);
        }
    }

    /// <summary>
    /// Performs a clustering process, assigning objects to clusters
    /// and recalculating centroids until convergence
    /// or the maximum number of iterations is reached.
    /// </summary>
    private void PerformClustering(List<CalculationNode> objects)
    {
        for (int iteration = 0; iteration < settings.AlgorithmSettings.MaxIterations; ++iteration)
        {
            clusters.ForEach(c => c.Nodes.Clear());

            if (!TryAssignObjectsToClusters(objects))
                break;

            RecalculateCentroids();
        }
    }

    /// <summary>
    /// Attempts to assign each object to the nearest cluster.
    /// Returns true if any object assignment changes.
    /// </summary>
    private bool TryAssignObjectsToClusters(List<CalculationNode> objects)
    {
        var assignmentsChanged = false;

        foreach (var obj in objects)
        {
            int nearestClusterIndex = GetNearestClusterIndex(obj);
            clusters[nearestClusterIndex].AddNode(obj);

            if (objectClusterMap.TryGetValue(obj, out int previousClusterIndex) ||
                previousClusterIndex != nearestClusterIndex)
            {
                continue;
            }

            objectClusterMap[obj] = nearestClusterIndex;
            assignmentsChanged = true;
        }

        return assignmentsChanged;
    }

    /// <summary>
    /// Calculates the index of the cluster that is closest to the object
    /// based on the distance to the cluster's centroid.
    /// </summary>
    private int GetNearestClusterIndex(CalculationNode node)
    {
        var clusterIndex = 0;
        var minDistance = double.MaxValue;
        
        for (int i = 0; i < clusters.Count; ++i)
        {
            var distance = distanceCalculator.Calculate(node, clusters[i].Centroid);

            if (distance >= minDistance)
                continue;

            minDistance = distance;
            clusterIndex = i;
        }

        return clusterIndex;
    }

    /// <summary>
    /// Recalculates the centroid of each cluster based on the current set of assigned objects.
    /// </summary>
    public void RecalculateCentroids()
    {
        foreach (var cluster in clusters)
        {
            cluster.Centroid = centroidCalculator.Recalculate(cluster.Centroid, cluster.Nodes);
        }
    }
}
