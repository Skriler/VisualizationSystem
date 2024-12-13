namespace VisualizationSystem.Models.Storages.Clusters;

public class AgglomerativeCluster : Cluster
{
    public bool IsMerged { get; set; }

    public double[] AverageParameters { get; private set; }

    public AgglomerativeCluster(NormalizedNode normalizedNode)
    {
        IsMerged = false;
        AverageParameters = normalizedNode.NormalizedParameters.ToArray();

        Nodes.Add(normalizedNode.Node);
    }

    public void UpdateAverageParameters(AgglomerativeCluster mergedCluster)
    {
        if (Nodes.Count == 0)
            return;

        var currentNodesCount = Nodes.Count;
        var mergedNodesCount = mergedCluster.Nodes.Count;
        var totalNodes = currentNodesCount + mergedNodesCount;

        for (int i = 0; i < AverageParameters.Length; ++i)
        {
            var weightedSum = (AverageParameters[i] * currentNodesCount) + (mergedCluster.AverageParameters[i] * mergedNodesCount);

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