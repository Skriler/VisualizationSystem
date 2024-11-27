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

        AddNodesToGraph(graph, similarityResults);
        AddEdgesToGraph(graph, similarityResults);

        return graph;
    }

    private void AddNodesToGraph(Graph graph, List<NodeSimilarityResult> similarityResults)
    {
        NodeDataMap.Clear();

        foreach (var similarityResult in similarityResults)
        {
            var currentNodeName = similarityResult.Node.Name;
            var nodeColor = GetNodeColor(currentNodeName);

            AddNode(graph, currentNodeName, nodeColor);

            NodeDataMap[currentNodeName] = similarityResult;
        }
    }

    private void AddEdgesToGraph(Graph graph, List<NodeSimilarityResult> similarityResults)
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

                AddEdge(graph, currentNodeName, similarNodeName);
                addedEdges.Add(edgeKey);
                addedEdges.Add(edgeKeyReversed);
            }
        }
    }

    private Node AddNode(Graph graph, string nodeName, SystemColor nodeColor)
    {
        var node = graph.FindNode(nodeName);

        if (node != null)
            return node;

        node = graph.AddNode(nodeName);
        node.LabelText = nodeName;
        node.Attr.FillColor = new MsaglColor(nodeColor.R, nodeColor.G, nodeColor.B);

        return node;
    }

    private Edge AddEdge(Graph graph, string firstNodeName, string secondNodeName)
    {
        var edge = graph.AddEdge(firstNodeName, secondNodeName);

        edge.Attr.ArrowheadAtSource = ArrowStyle.None;
        edge.Attr.ArrowheadAtTarget = ArrowStyle.None;

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
}
