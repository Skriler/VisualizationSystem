using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.MDS;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using MsaglColor = Microsoft.Msagl.Drawing.Color;
using SystemColor = System.Drawing.Color;

namespace VisualizationSystem.Services.Utilities;

public class GraphBuilder
{
    private readonly Random random;
    private readonly Dictionary<string, SystemColor> nodeColors = new();

    public UserSettings Settings { get; private set; }

    public Dictionary<string, NodeSimilarityResult> NodeDataMap { get; } = new();

    public GraphBuilder()
    {
        random = new Random();
    }

    public void UpdateSettings(UserSettings settings) => Settings = settings;

    public Graph BuildGraph(string name, List<NodeSimilarityResult> similarityResults)
    {
        var graph = new Graph(name)
        {
            LayoutAlgorithmSettings = new MdsLayoutSettings(),
        };

        AddNodes(graph, similarityResults);
        AddEdges(graph, similarityResults);

        var clusters = CreateClusters(similarityResults);
        AddClusters(graph, clusters, similarityResults.Count);

        return graph;
    }

    private void AddNodes(Graph graph, List<NodeSimilarityResult> similarityResults)
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

    private void AddEdges(Graph graph, List<NodeSimilarityResult> similarityResults)
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

    private List<Cluster> CreateClusters(List<NodeSimilarityResult> similarityResults, float thresholdBoost = 10f)
    {
        var clusterThreshold = Math.Min(100, Settings.MinSimilarityPercentage + thresholdBoost);

        var clusters = new List<Cluster>();
        var visitedNodes = new HashSet<string>();

        foreach (var similarityResult in similarityResults)
        {
            if (visitedNodes.Contains(similarityResult.Node.Name))
                continue;

            var cluster = new Cluster();
            cluster.AddNode(similarityResult.Node);
            visitedNodes.Add(similarityResult.Node.Name);

            foreach (var similarNode in similarityResult.SimilarNodes)
            {
                if (visitedNodes.Contains(similarNode.Node.Name))
                    continue;

                if (similarNode.SimilarityPercentage < clusterThreshold)
                    continue;

                cluster.AddNode(similarNode.Node);
                visitedNodes.Add(similarNode.Node.Name);
            }

            clusters.Add(cluster);
        }

        return clusters;
    }

    private void AddClusters(Graph graph, List<Cluster> clusters, int totalNodeCount)
    {
        foreach (var cluster in clusters)
        {
            var subgraph = new Subgraph(cluster.Id.ToString());
            subgraph.Attr.Shape = Shape.Circle;
            subgraph.Attr.LabelMargin = cluster.Id;
            subgraph.Attr.Padding = 2;
            subgraph.Attr.FillColor = MsaglColor.AliceBlue;

            foreach (var node in cluster.Nodes)
            {
                var nodeName = node.Name;

                if (graph.FindNode(nodeName) is not Node graphNode)
                    continue;

                subgraph.AddNode(graphNode);
                // TODO: Add Edges
            }

            graph.SubgraphMap.Add(cluster.Id.ToString(), subgraph);
        }
    }

    private Node AddNode(Graph graph, string nodeName, SystemColor nodeColor, int edgesCount, int maxEdges)
    {
        var node = graph.FindNode(nodeName);

        if (node != null)
            return node;

        node = graph.AddNode(nodeName);
        node.LabelText = nodeName;
        node.Attr.FillColor = new MsaglColor(nodeColor.R, nodeColor.G, nodeColor.B);

        node.Attr.LabelMargin = (int)GetNodeSize(edgesCount, maxEdges);

        return node;
    }

    private Edge AddEdge(Graph graph, string firstNodeName, string secondNodeName, float similarityPercentage)
    {
        var edge = graph.AddEdge(firstNodeName, secondNodeName);

        edge.Attr.ArrowheadAtSource = ArrowStyle.None;
        edge.Attr.ArrowheadAtTarget = ArrowStyle.None;

        //edge.Attr.Length = 1.0 / similarityPercentage;

        edge.Attr.Color = CalculateEdgeColor(similarityPercentage, Settings.MinSimilarityPercentage);

        //edge.LabelText = $"{similarityPercentage:F1}%";

        return edge;
    }

    private SystemColor GetNodeColor(string nodeName)
    {
        if (nodeColors.TryGetValue(nodeName, out SystemColor nodeColor))
            return nodeColor;

        var newColor = GetNewRandomColor();
        nodeColors[nodeName] = newColor;

        return newColor;
    }

    private SystemColor GetNewRandomColor()
    {
        SystemColor randColor;

        do
        {
            randColor = SystemColor.FromArgb(
                random.Next(256),
                random.Next(256),
                random.Next(256)
                );
        } while (nodeColors.Values.Contains(randColor));

        return randColor;
    }

    private MsaglColor CalculateEdgeColor(float similarityPercentage, float minSimilarityThreshold)
    {
        var normalizedSimilarity = (similarityPercentage - minSimilarityThreshold) / (100 - minSimilarityThreshold);

        // Interpolation between red (when the closest is 0) and green (when the closest is 100)
        var red = (byte)(255 * (1 - normalizedSimilarity)); // Red decreases from 255 to 0
        var green = (byte)(255 * normalizedSimilarity);     // Green increases from 0 to 255
        const byte blue = 0;                                // Blue stays at 0

        return new MsaglColor(red, green, blue);
    }

    private double GetNodeSize(int connectionCount, int maxConnections)
    {
        return 1 + (connectionCount / (double)maxConnections) * 20;
    }
}
