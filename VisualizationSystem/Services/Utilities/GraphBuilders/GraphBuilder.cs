using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;

namespace VisualizationSystem.Services.Utilities.GraphBuilders;

public abstract class GraphBuilder<TGraph> : IGraphBuilder<TGraph>
{
    protected readonly Random random = new();
    protected readonly Dictionary<string, Color> nodeColors = new();

    public Dictionary<string, NodeSimilarityResult> NodeDataMap { get; } = new();
    public UserSettings Settings { get; set; } = new();

    public abstract TGraph Build(string name, List<NodeSimilarityResult> similarityResults, List<Cluster> clusters);

    protected virtual void AddNodes(TGraph graph, List<NodeSimilarityResult> similarityResults)
    {
        NodeDataMap.Clear();

        foreach (var similarityResult in similarityResults)
        {
            var currentNodeName = similarityResult.Node.Name;
            var nodeColor = GetNodeColor(currentNodeName);

            AddNode(
                graph, 
                currentNodeName, 
                nodeColor, 
                similarityResult.SimilarNodesAboveThreshold, 
                similarityResult.SimilarNodes.Count
                );
            
            NodeDataMap[currentNodeName] = similarityResult;
        }
    }

    protected virtual void AddEdges(TGraph graph, List<NodeSimilarityResult> similarityResults)
    {
        var addedEdges = new HashSet<(string, string)>();

        foreach (var similarityResult in similarityResults)
        {
            var currentNodeName = similarityResult.Node.Name;

            foreach (var similarNode in similarityResult.SimilarNodes)
            {
                if (similarNode.SimilarityPercentage < Settings.MinSimilarityPercentage)
                    continue;

                var similarNodeName = similarNode.Node.Name;
                var edgeKey = (currentNodeName, similarNodeName);
                var edgeKeyReversed = (similarNodeName, currentNodeName);

                if (addedEdges.Contains(edgeKey) || addedEdges.Contains(edgeKeyReversed))
                    continue;

                AddEdge(graph, currentNodeName, similarNodeName, similarNode.SimilarityPercentage);
                addedEdges.Add(edgeKey);
                addedEdges.Add(edgeKeyReversed);
            }
        }
    }

    protected Color GetNodeColor(string nodeName)
    {
        if (nodeColors.TryGetValue(nodeName, out Color nodeColor))
            return nodeColor;

        var newColor = GetNewRandomColor();
        nodeColors[nodeName] = newColor;

        return newColor;
    }

    protected Color GetNewRandomColor()
    {
        Color randColor;

        do
        {
            randColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        } while (nodeColors.Values.Contains(randColor));

        return randColor;
    }

    protected Color CalculateEdgeColor(float similarityPercentage, float minSimilarityThreshold)
    {
        var normalizedSimilarity = (similarityPercentage - minSimilarityThreshold) / (100 - minSimilarityThreshold);

        // Interpolation between red (when the closest is 0) and green (when the closest is 100)
        var red = (byte)(255 * (1 - normalizedSimilarity)); // Red decreases from 255 to 0
        var green = (byte)(255 * normalizedSimilarity);     // Green increases from 0 to 255
        const byte blue = 0;                                // Blue stays at 0

        return Color.FromArgb(red, green, blue);
    }

    protected double GetNodeSize(int connectionCount, int maxConnections)
    {
        return 1 + connectionCount / (double)maxConnections * 20;
    }

    protected abstract void AddClusters(TGraph graph, List<Cluster> clusters);
    protected abstract void AddNode(TGraph graph, string nodeName, Color nodeColor, int edgesCount, int maxEdges);
    protected abstract void AddEdge(TGraph graph, string firstNodeName, string secondNodeName, float similarityPercentage);
}
