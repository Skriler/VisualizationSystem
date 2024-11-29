using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.Models.Storages;

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
