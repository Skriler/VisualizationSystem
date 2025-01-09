using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Services.Utilities.Settings;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class DBSCANClusterer : BaseClusterer
{
    private HashSet<NodeObject> visitedNodes;

    public DBSCANClusterer(
        DataNormalizer dataNormalizer,
        MetricDistanceCalculator distanceCalculator,
        ISettingsSubject settingsSubject
        )
        : base(dataNormalizer, distanceCalculator, settingsSubject)
    { }

    public override async Task<List<Cluster>> ClusterAsync(NodeTable nodeTable)
    {
        var nodes = await dataNormalizer.GetNormalizedNodesAsync(nodeTable);

        var clusters = new List<Cluster>();
        visitedNodes = new HashSet<NodeObject>();

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

        return clusters;
    }

    private List<NodeObject> GetNeighbors(NodeObject node, List<NodeObject> nodes)
    {
        return nodes
            .Where(other => IsNeighbor(node, other))
            .ToList();
    }

    private bool IsNeighbor(NodeObject node, NodeObject other)
    {
        //var distance = distanceCalculator.CalculateEuclidean(node.NormalizedParameters, other.NormalizedParameters);
        //return distance <= settings.AlgorithmSettings.Epsilon;
        return false;
    }

    private void ExpandCluster(
        Cluster cluster,
        NodeObject node,
        List<NodeObject> neighbors,
        List<NodeObject> nodes
        )
    {
        cluster.AddNode(node);

        var сonnectedNodes = new Queue<NodeObject>(neighbors);

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
}
