using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.Factories;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Services.Utilities.Settings;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class DBSCANClusterer : BaseClusterer
{
    private HashSet<CalculationNode> visitedNodes;

    protected override ClusterAlgorithm Algorithm => ClusterAlgorithm.DBSCAN;

    public DBSCANClusterer(
        DataNormalizer dataNormalizer,
        DistanceCalculatorFactory distanceCalculatorFactory,
        ISettingsSubject settingsSubject
        )
        : base(dataNormalizer, distanceCalculatorFactory, settingsSubject)
    { }

    public override async Task<List<Cluster>> ClusterAsync(NodeTable nodeTable)
    {
        var nodes = await dataNormalizer.GetCalculationNodesAsync(nodeTable, settings.ParameterStates);

        var clusters = new List<Cluster>();
        visitedNodes = new HashSet<CalculationNode>();

        foreach (var node in nodes)
        {
            if (visitedNodes.Contains(node))
                continue;

            visitedNodes.Add(node);
            var neighbors = GetNeighbors(node, nodes);

            if (neighbors.Count < settings.AlgorithmSettings.MinPoints)
                continue;

            var cluster = new Cluster();
            clusters.Add(cluster);

            ExpandCluster(cluster, node, neighbors, nodes);
        }

        var noiseCluster = GetNoiseCluster(nodes, clusters);
        clusters.Add(noiseCluster);

        return clusters;
    }

    private List<CalculationNode> GetNeighbors(CalculationNode node, List<CalculationNode> nodes)
    {
        return nodes
            .Where(other => IsNeighbor(node, other))
            .ToList();
    }

    private bool IsNeighbor(CalculationNode node, CalculationNode other)
    {
        var distance = distanceCalculator.Calculate(node, other);
        return distance <= settings.AlgorithmSettings.Epsilon;
    }

    private void ExpandCluster(
        Cluster cluster,
        CalculationNode node,
        List<CalculationNode> neighbors,
        List<CalculationNode> nodes
        )
    {
        cluster.AddNode(node);

        var сonnectedNodes = new Queue<CalculationNode>(neighbors);

        while (сonnectedNodes.Count > 0)
        {
            var neighbor = сonnectedNodes.Dequeue();

            if (visitedNodes.Contains(neighbor))
                continue;

            visitedNodes.Add(neighbor);
            var neighborNeighbors = GetNeighbors(neighbor, nodes);

            if (neighborNeighbors.Count >= settings.AlgorithmSettings.MinPoints)
            {
                foreach (var newNeighbor in neighborNeighbors)
                {
                    if (!cluster.Nodes.Contains(newNeighbor) && !сonnectedNodes.Contains(newNeighbor))
                    {
                        сonnectedNodes.Enqueue(newNeighbor);
                    }
                }
            }

            if (!cluster.Nodes.Contains(neighbor))
            {
                cluster.AddNode(neighbor);
            }
        }
    }

    private Cluster GetNoiseCluster(List<CalculationNode> nodes, List<Cluster> clusters)
    {
        var noiseNodes = nodes
            .Where(node => !clusters.Any(cluster => cluster.Nodes.Contains(node)))
            .ToList();

        var noiseCluster = new Cluster()
        {
            Type = ClusterType.Noise,
        };
        noiseNodes.ForEach(noiseCluster.AddNode);

        return noiseCluster;
    }
}
