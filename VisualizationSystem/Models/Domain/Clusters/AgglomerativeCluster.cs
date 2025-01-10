using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Models.Domain.Clusters;

public class AgglomerativeCluster : Cluster
{
    public bool IsMerged { get; set; }
    //public List<WeightedParameter> AverageParameters { get; private set; }

    public AgglomerativeCluster(NodeObject node)
    {
        IsMerged = false;
        //AverageParameters = node.NormalizedParameters
        //    .ConvertAll(nn => new WeightedParameter(nn.Value, nn.NormalizedParameterState.Weight));

        Nodes.Add(node);
    }

    public void UpdateAverageParameters(AgglomerativeCluster mergedCluster)
    {
        if (Nodes.Count == 0)
            return;

        var currentNodesCount = Nodes.Count;
        var mergedNodesCount = mergedCluster.Nodes.Count;
        var totalNodes = currentNodesCount + mergedNodesCount;

        //for (int i = 0; i < AverageParameters.Count; ++i)
        //{
        //    var weightedSum = AverageParameters[i].Value * currentNodesCount +
        //        mergedCluster.AverageParameters[i].Value * mergedNodesCount;

        //    AverageParameters[i].Value = weightedSum / totalNodes;
        //}
    }

    public void Merge(AgglomerativeCluster mergedCluster)
    {
        Nodes.AddRange(mergedCluster.Nodes);
        mergedCluster.IsMerged = true;

        UpdateAverageParameters(mergedCluster);
    }
}