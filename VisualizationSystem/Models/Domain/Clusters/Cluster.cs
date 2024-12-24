using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Models.Domain.Clusters;

public class Cluster
{
    private static int nextId = 0;

    public int Id { get; }
    public List<NodeObject> Nodes { get; } = new();

    public Cluster()
    {
        Id = nextId++;
    }

    public void AddNode(NodeObject node)
    {
        if (Nodes.Contains(node))
            return;

        Nodes.Add(node);
    }
}
