﻿using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.Normalizers;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class AgglomerativeClusterer : BaseClusterer
{
    private List<AgglomerativeCluster> agglomerativeClusters;

    public AgglomerativeClusterer(DataNormalizer dataNormalizer, MetricDistanceCalculator distanceCalculator)
        : base(dataNormalizer, distanceCalculator)
    { }

    public override async Task<List<Cluster>> ClusterAsync(NodeTable nodeTable)
    {
        var normalizedNodes = await dataNormalizer.GeNormalizedNodesAsync(nodeTable);

        agglomerativeClusters = normalizedNodes
            .Select(n => new AgglomerativeCluster(n))
            .ToList();

        while (agglomerativeClusters.Count(c => !c.IsMerged) > 1)
        {
            var similarCluster = FindMostSimilarClusters();

            if (similarCluster.Similarity < AlgorithmSettings.Threshold)
                break;

            agglomerativeClusters[similarCluster.FirstClusterId]
                .Merge(agglomerativeClusters[similarCluster.SecondClusterId]);
        }

        return agglomerativeClusters.Where(c => !c.IsMerged).Cast<Cluster>().ToList();
    }

    private ClusterSimilarityResult FindMostSimilarClusters()
    {
        var clusterSimilarity = new ClusterSimilarityResult();

        for (int i = 0; i < agglomerativeClusters.Count; ++i)
        {
            if (agglomerativeClusters[i].IsMerged)
                continue;

            for (int j = i + 1; j < agglomerativeClusters.Count; ++j)
            {
                if (agglomerativeClusters[j].IsMerged)
                    continue;

                var similarity = distanceCalculator.CalculateCosine(
                    agglomerativeClusters[i].AverageParameters,
                    agglomerativeClusters[j].AverageParameters
                    );

                if (similarity <= clusterSimilarity.Similarity)
                    continue;

                clusterSimilarity.Update(i, j, similarity);
            }
        }

        return clusterSimilarity;
    }
}
