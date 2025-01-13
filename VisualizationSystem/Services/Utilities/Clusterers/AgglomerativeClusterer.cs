using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.DistanceCalculators;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Services.Utilities.Settings;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class AgglomerativeClusterer : BaseClusterer
{
    private List<AgglomerativeCluster> agglomerativeClusters;

    public AgglomerativeClusterer(
        DataNormalizer dataNormalizer,
        IDistanceCalculator distanceCalculator,
        ISettingsSubject settingsSubject
        )
        : base(dataNormalizer, distanceCalculator, settingsSubject)
    { }

    public override async Task<List<Cluster>> ClusterAsync(NodeTable nodeTable)
    {
        var nodes = await dataNormalizer.GetCalculationNodesAsync(nodeTable);

        agglomerativeClusters = nodes
            .ConvertAll(n => new AgglomerativeCluster(n));

        while (agglomerativeClusters.Count(c => !c.IsMerged) > 1)
        {
            var similarCluster = FindMostSimilarClusters();

            if (similarCluster.Similarity < settings.AlgorithmSettings.Threshold)
                break;

            agglomerativeClusters[similarCluster.FirstClusterId]
                .Merge(agglomerativeClusters[similarCluster.SecondClusterId]);
        }

        return agglomerativeClusters
            .Where(c => !c.IsMerged)
            .Cast<Cluster>()
            .ToList();
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

                var similarity = GetMinimumDistance(
                    agglomerativeClusters[i],
                    agglomerativeClusters[j]
                    );

                if (similarity <= clusterSimilarity.Similarity)
                    continue;

                clusterSimilarity.Update(i, j, similarity);
            }
        }

        return clusterSimilarity;
    }

    private double GetAverageDistance(AgglomerativeCluster first, AgglomerativeCluster second)
    {
        return first.Nodes
            .SelectMany(firstNode => second.Nodes, distanceCalculator.Calculate)
            .Average();
    }

    private double GetMinimumDistance(AgglomerativeCluster first, AgglomerativeCluster second)
    {
        return first.Nodes
            .SelectMany(firstNode => second.Nodes, distanceCalculator.Calculate)
            .Min();

    }
}
