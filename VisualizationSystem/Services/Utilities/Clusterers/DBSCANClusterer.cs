﻿using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Nodes.Normalized;
using VisualizationSystem.Services.Utilities.Normalizers;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class DBSCANClusterer : BaseClusterer
{
    private HashSet<NormNode> visitedNodes;

    public DBSCANClusterer(DataNormalizer dataNormalizer, MetricDistanceCalculator distanceCalculator)
        : base(dataNormalizer, distanceCalculator)
    { }

    public override async Task<List<Cluster>> ClusterAsync(NodeTable nodeTable)
    {
        var normalizedNodes = await dataNormalizer.GeNormalizedNodesAsync(nodeTable);

        var clusters = new List<Cluster>();
        visitedNodes = new HashSet<NormNode>();

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

    private List<NormNode> GetNeighbors(NormNode node, List<NormNode> nodes)
    {
        return nodes
            .Where(other => IsNeighbor(node, other))
            .ToList();
    }

    private bool IsNeighbor(NormNode node, NormNode other)
    {
        var distance = distanceCalculator.CalculateEuclidean(node.NormParameters, other.NormParameters);
        return distance <= AlgorithmSettings.Epsilon;
    }

    private void ExpandCluster(
        Cluster cluster,
        NormNode node,
        List<NormNode> neighbors,
        List<NormNode> nodes
        )
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
}
