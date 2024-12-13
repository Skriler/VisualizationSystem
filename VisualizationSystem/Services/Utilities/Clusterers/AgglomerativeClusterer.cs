using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages.Clusters;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class AgglomerativeClusterer : IClusterize
{
    private readonly DataNormalizer dataNormalizer;

    public AgglomerativeClusterer(DataNormalizer dataNormalizer)
    {
        this.dataNormalizer = dataNormalizer;
    }

    public List<Cluster> Cluster(List<NodeObject> nodes, float minSimilarityThreshold)
    {
        var normalizedNodes = dataNormalizer.GetNormalizedNodes(nodes);

        var clusters = normalizedNodes
            .Select(n => new AgglomerativeCluster(n))
            .ToList();

        while (clusters.Count(c => !c.IsMerged) > 1)
        {
            var similarCluster = FindMostSimilarClusters(clusters);

            if (similarCluster.Similarity < minSimilarityThreshold)
                break;

            clusters[similarCluster.FirstClusterId]
                .Merge(clusters[similarCluster.SecondClusterId]);
        }

        return clusters.Where(c => !c.IsMerged).Cast<Cluster>().ToList();
    }

    private ClusterSimilarityResult FindMostSimilarClusters(List<AgglomerativeCluster> clusters)
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

                var similarity = GetCosineSimilarity(
                    clusters[i].AverageParameters, 
                    clusters[j].AverageParameters
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
        var dotProduct = firstParameters.Zip(secondParameters, (x, y) => x * y).Sum();
        var firstMagnitude = Math.Sqrt(firstParameters.Sum(x => x * x));
        var secondMagnitude = Math.Sqrt(secondParameters.Sum(x => x * x));

        return (float)(dotProduct / (firstMagnitude * secondMagnitude));
    }
}
