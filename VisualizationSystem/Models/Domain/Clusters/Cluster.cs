using VisualizationSystem.Models.Domain.Nodes;

namespace VisualizationSystem.Models.Domain.Clusters;

public class Cluster
{
    private static int nextId = 0;

    public int Id { get; }

    public List<CalculationNode> Nodes { get; } = new();

    public Cluster()
    {
        Id = nextId++;
    }

    public void AddNode(CalculationNode node)
    {
        if (Nodes.Contains(node))
            return;

        Nodes.Add(node);
    }
}
