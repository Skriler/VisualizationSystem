using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Models.Domain.Graphs;

public readonly struct CustomEdge
{
    public NodeObject Source { get; }
    public NodeObject Target { get; }

    public CustomEdge(NodeObject source, NodeObject target)
    {
        Source = source;
        Target = target;
    }

    public bool Equals(CustomEdge other)
    {
        return (Source == other.Source && Target == other.Target) ||
               (Source == other.Target && Target == other.Source);
    }
}
