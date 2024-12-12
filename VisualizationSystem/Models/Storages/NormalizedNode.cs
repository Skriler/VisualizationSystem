using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.Storages;

public class NormalizedNode
{
    public NodeObject Node { get; set; }
    public List<double> NormalizedParameters { get; set; }

    public NormalizedNode(NodeObject node)
    {
        Node = node;
        NormalizedParameters = new List<double>();
    }
}
