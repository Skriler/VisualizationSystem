using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.Factories;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Services.Utilities.Settings;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class DBSCANClusterer : BaseClusterer
{
    protected override ClusterAlgorithm Algorithm => ClusterAlgorithm.DBSCAN;

    /// <summary>
    /// A set of visited objects to prevent reprocessing.
    /// </summary>
    private HashSet<CalculationNode> visitedNodes = default!;

    /// <summary>
    /// A set of objects classified as noise.
    /// </summary>
    private HashSet<CalculationNode> noiseNodes = default!;

    public DBSCANClusterer(
        DataNormalizer dataNormalizer,
        DistanceCalculatorFactory distanceCalculatorFactory,
        ISettingsSubject settingsSubject
        )
        : base(dataNormalizer, distanceCalculatorFactory, settingsSubject)
    { }

    /// <summary>
    /// Performs a clustering process by iterating over all objects.
    /// For each unvisited object, it checks if it has enough neighbors to form a cluster.
    /// If it does, a new cluster is created, and the cluster is expanded by adding neighboring objects.
    /// Objects with insufficient neighbors are marked as noise.
    /// </summary>
    public override async Task<List<Cluster>> ClusterAsync(NodeTable nodeTable)
    {
        var nodes = await dataNormalizer.GetCalculationNodesAsync(nodeTable, settings.ParameterStates);

        var clusters = new List<Cluster>();
        visitedNodes = new HashSet<CalculationNode>();
        noiseNodes = new HashSet<CalculationNode>();

        foreach (var node in nodes)
        {
            if (visitedNodes.Contains(node))
                continue;

            visitedNodes.Add(node);

            var neighbors = GetNeighbors(node, nodes);

            if (neighbors.Count < settings.AlgorithmSettings.MinPoints)
            {
                noiseNodes.Add(node);
                continue;
            }

            var cluster = new Cluster();
            cluster.AddNode(node);
            noiseNodes.Remove(node);

            clusters.Add(cluster);

            ExpandCluster(cluster, neighbors, nodes);
        }

        //AddNoiseCluster(clusters);

        return clusters;
    }

    /// <summary>
    /// Retrieves the neighbors of a given object from the list of all objects.
    /// </summary>
    private List<CalculationNode> GetNeighbors(
        CalculationNode node,
        List<CalculationNode> nodes)
    {
        return nodes
            .Where(other => other != node && IsNeighbor(node, other))
            .ToList();
    }

    /// <summary>
    /// Checks if two objects are neighbors based on the distance metric.
    /// </summary>
    private bool IsNeighbor(CalculationNode node, CalculationNode other)
    {
        var distance = distanceCalculator.Calculate(node, other);
        return distance <= settings.AlgorithmSettings.Epsilon;
    }

    /// <summary>
    /// Expands the cluster by adding neighbors
    /// and their neighbors recursively.
    /// </summary>
    private void ExpandCluster(
        Cluster cluster,
        List<CalculationNode> neighbors,
        List<CalculationNode> nodes
        )
    {
        var neighborsToProcess = new Queue<CalculationNode>(neighbors);

        while (neighborsToProcess.Count > 0)
        {
            var currentNeighbor = neighborsToProcess.Dequeue();

            if (visitedNodes.Contains(currentNeighbor))
                continue;

            cluster.AddNode(currentNeighbor);
            visitedNodes.Add(currentNeighbor);
            noiseNodes.Remove(currentNeighbor);

            var currentNeighbors = GetNeighbors(currentNeighbor, nodes);

            if (currentNeighbors.Count < settings.AlgorithmSettings.MinPoints)
                continue;

            foreach (var neighbor in currentNeighbors)
            {
                if (visitedNodes.Contains(neighbor) ||
                    neighborsToProcess.Contains(neighbor))
                {
                    continue;
                }

                neighborsToProcess.Enqueue(neighbor);
            }
        }
    }

    /// <summary>
    /// Adds a noise cluster to the list of clusters
    /// if there are any noise objects.
    /// </summary>
    private void AddNoiseCluster(List<Cluster> clusters)
    {
        if (noiseNodes.Count == 0)
            return;

        var noiseCluster = new Cluster();

        foreach (var noiseNode in noiseNodes)
        {
            noiseCluster.AddNode(noiseNode);
        }

        clusters.Add(noiseCluster);
    }
}
