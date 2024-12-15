using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Storages.Clusters;
using VisualizationSystem.Services.Utilities.Normalizers;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class AgglomerativeClusterer : IClusterize
{
    private readonly DataNormalizer dataNormalizer;

    private readonly float minSimilarityThreshold;

    public UserSettings Settings { get; set; } = new();

    private List<AgglomerativeCluster> agglomerativeClusters;


    public AgglomerativeClusterer(DataNormalizer dataNormalizer, float minSimilarityThreshold = 0.75f)
    {
        this.dataNormalizer = dataNormalizer;

        this.minSimilarityThreshold = minSimilarityThreshold;
    }

    public List<Cluster> Cluster(List<NodeObject> nodes)
    {
        var normalizedNodes = dataNormalizer.GetNormalizedNodes(nodes);

        agglomerativeClusters = normalizedNodes
            .Select(n => new AgglomerativeCluster(n))
            .ToList();

        while (agglomerativeClusters.Count(c => !c.IsMerged) > 1)
        {
            var similarCluster = FindMostSimilarClusters();

            if (similarCluster.Similarity < minSimilarityThreshold)
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

                var similarity = GetCosineSimilarity(
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

    private float GetCosineSimilarity(double[] firstParameters, double[] secondParameters)
    {
        if (firstParameters.Length != secondParameters.Length)
            throw new InvalidOperationException("Parameters must be the same length");

        var dotProduct = firstParameters.Zip(secondParameters, (x, y) => x * y).Sum();
        var firstMagnitude = Math.Sqrt(firstParameters.Sum(x => x * x));
        var secondMagnitude = Math.Sqrt(secondParameters.Sum(x => x * x));

        return (float)(dotProduct / (firstMagnitude * secondMagnitude));
    }
}
