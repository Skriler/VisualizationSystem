using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.Factories;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Services.Utilities.Settings;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class AgglomerativeClusterer : BaseClusterer
{
    protected override ClusterAlgorithm Algorithm => ClusterAlgorithm.HierarchicalAgglomerative;

    private List<AgglomerativeCluster> clusters = default!;

    public AgglomerativeClusterer(
        DataNormalizer dataNormalizer,
        DistanceCalculatorFactory distanceCalculatorFactory,
        ISettingsSubject settingsSubject
        )
        : base(dataNormalizer, distanceCalculatorFactory, settingsSubject)
    { }

    public override async Task<List<Cluster>> ClusterAsync(NodeTable nodeTable)
    {
        var nodes = await dataNormalizer.GetCalculationNodesAsync(nodeTable, settings.ParameterStates);

        clusters = nodes.ConvertAll(n => new AgglomerativeCluster(n));
        PerformClustering();

        return clusters
            .Where(c => !c.IsMerged)
            .Cast<Cluster>()
            .ToList();
    }

    /// <summary>
    /// Performs a clustering process, merging the most similar clusters
    /// until convergence or a threshold is reached.
    /// </summary>
    private void PerformClustering()
    {
        while (clusters.Count(c => !c.IsMerged) > 1)
        {
            var similarCluster = FindMostSimilarClusters();

            if (similarCluster.Similarity > settings.AlgorithmSettings.Threshold)
                break;

            clusters[similarCluster.FirstClusterId]
                .Merge(clusters[similarCluster.SecondClusterId]);
        }
    }

    /// <summary>
    /// Finds the cluster pair with the highest similarity.
    /// </summary>
    private ClusterSimilarityResult FindMostSimilarClusters()
    {
        var clusterSimilarity = new ClusterSimilarityResult();

        for (int i = 0; i < clusters.Count; ++i)
        {
            if (clusters[i].IsMerged)
                continue;

            for (int j = i + 1; j < clusters.Count; ++j)
            {
                if (clusters[j].IsMerged)
                    continue;

                var similarity = GetAverageDistance(
                    clusters[i],
                    clusters[j]
                    );

                if (similarity > clusterSimilarity.Similarity)
                    continue;

                clusterSimilarity.Update(i, j, similarity);
            }
        }

        return clusterSimilarity;
    }

    /// <summary>
    /// Calculates the average distance between two clusters based on their objects.
    /// </summary>
    private double GetAverageDistance(AgglomerativeCluster first, AgglomerativeCluster second)
    {
        return first.Nodes
            .SelectMany(firstNode => second.Nodes, distanceCalculator.Calculate)
            .Average();
    }

    /// <summary>
    /// Calculates the minimum distance between two clusters based on their objects.
    /// </summary>
    private double GetMinimumDistance(AgglomerativeCluster first, AgglomerativeCluster second)
    {
        return first.Nodes
            .SelectMany(firstNode => second.Nodes, distanceCalculator.Calculate)
            .Min();
    }
}
