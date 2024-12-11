using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.Storages;
public class NormalizedNodeData
{
    public NodeObject Node { get; set; }
    public double[] NormalizedParameters { get; set; }

    public NormalizedNodeData(NodeObject node, double[] normalizedParameters)
    {
        Node = node;
        NormalizedParameters = normalizedParameters;
    }
}
