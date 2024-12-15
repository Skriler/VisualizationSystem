using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Storages.Clusters;
using VisualizationSystem.Models.Storages.Nodes;
using VisualizationSystem.Services.Utilities.Normalizers;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class DBSCANClusterer : IClusterize
{
    private readonly DataNormalizer dataNormalizer;
    private readonly double eps;
    private readonly int minPts;

    public UserSettings Settings { get; set; } = new();

    private HashSet<NormalizedNode> visitedNodes;

    public DBSCANClusterer(DataNormalizer dataNormalizer, double eps = 4, int minPts = 4)
    {
        this.dataNormalizer = dataNormalizer;

        this.eps = eps;
        this.minPts = minPts;
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

            if (neighbors.Count < minPts)
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

        return distance <= eps;
    }

    private void ExpandCluster(
        Cluster cluster,
        NormalizedNode node,
        List<NormalizedNode> neighbors,
        List<NormalizedNode> nodes)
    {
        cluster.AddNode(node.Node);

        foreach (var neighbor in neighbors)
        {
            if (cluster.Nodes.Contains(neighbor.Node))
                continue;

            cluster.AddNode(neighbor.Node);

            if (visitedNodes.Contains(neighbor))
                continue;

            visitedNodes.Add(neighbor);

            var neighborNeighbors = GetNeighbors(neighbor, nodes);

            if (neighborNeighbors.Count < minPts)
                continue;

            ExpandCluster(cluster, neighbor, neighborNeighbors, nodes);
        }
    }

    private double GetMixedDistance(List<double> firstParameters, List<double> secondParameters)
    {
        if (firstParameters.Count != secondParameters.Count)
            throw new InvalidOperationException("Parameters must be the same length");

        double distance = 0;

        for (int i = 0; i < firstParameters.Count; ++i)
        {
            var diff = firstParameters[i] - secondParameters[i];
            distance += diff * diff;
        }

        return Math.Sqrt(distance);
    }
}
