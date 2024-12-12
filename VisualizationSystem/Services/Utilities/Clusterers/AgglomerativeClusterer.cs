using VisualizationSystem.Models.Storages;
using VisualizationSystem.Models.Storages.Clusters;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public class AgglomerativeClusterer : IClusterize
{
    private Dictionary<string, int> nodeToIndex = new();
    private float[,] similarityMatrix = {};

    public List<Cluster> Cluster(List<NodeSimilarityResult> similarityResults, float minSimilarityThreshold)
    {
        var clusters = similarityResults
            .Select(sr => new Cluster() { Nodes = { sr.Node } })
            .ToList();

        var isMerged = new bool[clusters.Count];
        BuildNodeToIndexMapping(similarityResults);
        BuildSimilarityMatrix(similarityResults);

        while (clusters.Count > 1)
        {
            var similarCluster = FindMostSimilarClusters(clusters, isMerged);

            if (similarCluster.Similarity < minSimilarityThreshold)
                break;

            clusters[similarCluster.FirstClusterId].Merge(clusters[similarCluster.SecondClusterId]);
            isMerged[similarCluster.SecondClusterId] = true;

            UpdateSimilarityMatrix(
                clusters,
                similarCluster.FirstClusterId,
                similarCluster.SecondClusterId
                );
        }

        clusters = clusters.Where((cluster, index) => !isMerged[index]).ToList();

        return clusters;
    }

    private void BuildNodeToIndexMapping(List<NodeSimilarityResult> similarityResults)
    {
        nodeToIndex = similarityResults
            .Select((result, index) => new { result.Node.Name, index })
            .ToDictionary(n => n.Name, n => n.index);
    }

    private void BuildSimilarityMatrix(List<NodeSimilarityResult> similarityResults)
    {
        var n = similarityResults.Count;
        similarityMatrix = new float[n, n];

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                if (i == j)
                {
                    similarityMatrix[i, j] = 1;
                    continue;
                }

                var similarNode = similarityResults[i].SimilarNodes
                    .FirstOrDefault(sn => sn.Node.Name == similarityResults[j].Node.Name);

                similarityMatrix[i, j] = similarNode?.SimilarityPercentage ?? 0;
            }
        }
    }

    private ClusterSimilarityResult FindMostSimilarClusters(List<Cluster> clusters, bool[] isMerged)
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

                var similarity = ComputeClusterSimilarity(clusters[i], clusters[j]);

                if (similarity <= clusterSimilarity.Similarity)
                    continue;

                clusterSimilarity.Update(i, j, similarity);
            }
        }

        return clusterSimilarity;
    }

    private float ComputeClusterSimilarity(Cluster firstCluster, Cluster secondCluster)
    {
        var firstNames = firstCluster.Nodes.Select(node => node.Name).ToList();
        var secondNames = secondCluster.Nodes.Select(node => node.Name).ToList();
        var count = firstNames.Count * secondNames.Count;

        var totalSimilarity = (
            from firstName in firstNames
            from secondName in secondNames
            let firstIndex = nodeToIndex[firstName]
            let secondIndex = nodeToIndex[secondName]
            select similarityMatrix[firstIndex, secondIndex]
            ).Sum();

        return totalSimilarity / count;
    }

    private void UpdateSimilarityMatrix(List<Cluster> clusters, int mergedCluster, int removedCluster)
    {
        for (int i = 0; i < clusters.Count; i++)
        {
            if (i == mergedCluster || i == removedCluster)
                continue;

            var newSimilarity = ComputeClusterSimilarity(clusters[i], clusters[mergedCluster]);

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
