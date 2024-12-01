using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class AgglomerativeClusterer : IClusterize
{
    public UserSettings Settings { get; private set; }

    public void UpdateSettings(UserSettings settings) => Settings = settings;

    public List<Cluster> Cluster(List<NodeSimilarityResult> similarityResults, float minSimilarityThreshold)
    {
        var clusters = similarityResults
            .Select(sr => new Cluster() { Nodes = { sr.Node } })
            .ToList();

        var isMerged = new bool[clusters.Count];
        var similarityMatrix = BuildSimilarityMatrix(similarityResults);

        while (clusters.Count > 1)
        {
            var similarCluster = FindMostSimilarClusters(similarityMatrix, clusters, isMerged);

            if (similarCluster.Similarity < minSimilarityThreshold)
                break;

            clusters[similarCluster.FirstClusterId].Merge(clusters[similarCluster.SecondClusterId]);
            isMerged[similarCluster.SecondClusterId] = true;

            UpdateSimilarityMatrix(
                similarityMatrix,
                clusters,
                similarCluster.FirstClusterId,
                similarCluster.SecondClusterId
                );
        }

        clusters = clusters.Where((cluster, index) => !isMerged[index]).ToList();

        return clusters;
    }

    private float[,] BuildSimilarityMatrix(List<NodeSimilarityResult> similarityResults)
    {
        var n = similarityResults.Count;
        var matrix = new float[n, n];

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                if (i == j)
                {
                    matrix[i, j] = 1;
                    continue;
                }

                var similarNode = similarityResults[i].SimilarNodes
                    .FirstOrDefault(sn => sn.Node == similarityResults[j].Node);

                matrix[i, j] = similarNode?.SimilarityPercentage ?? 0;
            }
        }

        return matrix;
    }

    private ClusterSimilarityResult FindMostSimilarClusters(float[,] similarityMatrix, List<Cluster> clusters, bool[] isMerged)
    {
        var clusterSimilarity = new ClusterSimilarityResult();

        for (int i = 0; i < clusters.Count; ++i)
        {
            if (isMerged[i])
                continue;

            for (int j = i + 1; j < clusters.Count; ++j)
            {
                if (isMerged[j])
                    continue;

                var similarity = ComputeClusterSimilarity(similarityMatrix, clusters[i], clusters[j]);

                if (similarity <= clusterSimilarity.Similarity)
                    continue;

                clusterSimilarity.Update(i, j, similarity);
            }
        }

        return clusterSimilarity;
    }

    private float ComputeClusterSimilarity(float[,] similarityMatrix, Cluster firstCluster, Cluster secondCluster)
    {
        var firstIds = firstCluster.Nodes.Select(node => node.Id).ToList();
        var secondIds = secondCluster.Nodes.Select(node => node.Id).ToList();
        var count = firstIds.Count * secondIds.Count;

        var totalSimilarity = (
            from firstId in firstIds
            from secondId in secondIds
            select similarityMatrix[firstId - 1, secondId - 1]
            ).Sum();

        return totalSimilarity / count;
    }

    private void UpdateSimilarityMatrix(float[,] similarityMatrix, List<Cluster> clusters, int mergedCluster, int removedCluster)
    {
        for (int i = 0; i < clusters.Count; i++)
        {
            if (i == mergedCluster || i == removedCluster)
                continue;

            var newSimilarity = ComputeClusterSimilarity(similarityMatrix, clusters[i], clusters[mergedCluster]);

            similarityMatrix[i, mergedCluster] = newSimilarity;
            similarityMatrix[mergedCluster, i] = newSimilarity;
        }

        for (int i = 0; i < clusters.Count; i++)
        {
            similarityMatrix[i, removedCluster] = -1;
            similarityMatrix[removedCluster, i] = -1;
        }
    }
}
