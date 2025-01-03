﻿using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Models.Domain.Clusters;

public class AgglomerativeCluster : Cluster
{
    public bool IsMerged { get; set; }
    public List<double> AverageParameters { get; private set; }

    public AgglomerativeCluster(NodeObject node)
    {
        IsMerged = false;
        AverageParameters = node.NormalizedParameters
            .Select(nn => nn.Value)
            .ToList();

        Nodes.Add(node);
    }

    public void UpdateAverageParameters(AgglomerativeCluster mergedCluster)
    {
        if (Nodes.Count == 0)
            return;

        var currentNodesCount = Nodes.Count;
        var mergedNodesCount = mergedCluster.Nodes.Count;
        var totalNodes = currentNodesCount + mergedNodesCount;

        for (int i = 0; i < AverageParameters.Count; ++i)
        {
            var weightedSum = AverageParameters[i] * currentNodesCount + mergedCluster.AverageParameters[i] * mergedNodesCount;

            AverageParameters[i] = weightedSum / totalNodes;
        }
    }

    public void Merge(AgglomerativeCluster mergedCluster)
    {
        Nodes.AddRange(mergedCluster.Nodes);
        mergedCluster.IsMerged = true;

        UpdateAverageParameters(mergedCluster);
    }
}