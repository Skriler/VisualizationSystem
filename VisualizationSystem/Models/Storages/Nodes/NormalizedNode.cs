using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Models.Storages.Nodes;

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
