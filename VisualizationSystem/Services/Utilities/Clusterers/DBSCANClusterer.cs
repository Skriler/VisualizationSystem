using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Nodes.Normalized;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Storages.Clusters;
using VisualizationSystem.Services.Utilities.Normalizers;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class DBSCANClusterer : IClusterize
{
    private readonly DataNormalizer dataNormalizer;
    private HashSet<NormalizedNode> visitedNodes;

    public ClusterAlgorithmSettings AlgorithmSettings { get; set; } = new();

    public DBSCANClusterer(DataNormalizer dataNormalizer)
    {
        this.dataNormalizer = dataNormalizer;
    }

    public List<Cluster> Cluster(List<NodeObject> nodes)
    {
        var normalizedNodes = dataNormalizer.GetNormalizedNodes(nodes);

        var clusters = new List<Cluster>();
        visitedNodes = new HashSet<NormalizedNode>();

        foreach (var node in normalizedNodes)
        {
            if (visitedNodes.Contains(node))
                continue;

            visitedNodes.Add(node);

            var neighbors = GetNeighbors(node, normalizedNodes);

            if (neighbors.Count < AlgorithmSettings.MinPoints)
                continue;

            var cluster = new Cluster();
            clusters.Add(cluster);

            ExpandCluster(cluster, node, neighbors, normalizedNodes);
        }

        return clusters;
    }

    private List<NormalizedNode> GetNeighbors(NormalizedNode node, List<NormalizedNode> nodes)
    {
        return nodes
            .Where(other => IsNeighbor(node, other))
            .ToList();
    }

    private bool IsNeighbor(NormalizedNode node, NormalizedNode other)
    {
        var distance = GetMixedDistance(node.NormalizedParameters, other.NormalizedParameters);

        return distance <= AlgorithmSettings.Epsilon;
    }

    private void ExpandCluster(
        Cluster cluster,
        NormalizedNode node,
        List<NormalizedNode> neighbors,
        List<NormalizedNode> nodes)
    {
        cluster.AddNode(node.NodeObject);

        foreach (var neighbor in neighbors)
        {
            if (cluster.Nodes.Contains(neighbor.NodeObject))
                continue;

            cluster.AddNode(neighbor.NodeObject);

            if (visitedNodes.Contains(neighbor))
                continue;

            visitedNodes.Add(neighbor);

            var neighborNeighbors = GetNeighbors(neighbor, nodes);

            if (neighborNeighbors.Count < AlgorithmSettings.MinPoints)
                continue;

            ExpandCluster(cluster, neighbor, neighborNeighbors, nodes);
        }
    }

    private double GetMixedDistance(List<NormalizedNodeParameter> firstParameters, List<NormalizedNodeParameter> secondParameters)
    {
        if (firstParameters.Count != secondParameters.Count)
            throw new InvalidOperationException("Parameters must be the same length");

        double distance = 0;

        for (int i = 0; i < firstParameters.Count; ++i)
        {
            var diff = firstParameters[i].Value - secondParameters[i].Value;
            distance += diff * diff;
        }

        return Math.Sqrt(distance);
    }
}
