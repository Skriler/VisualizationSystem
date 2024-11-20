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

    public UserSettings Settings { get; private set; }

    public Dictionary<string, NodeSimilarityResult> NodeDataMap { get; private set; }

    public GraphBuilder(UserSettings settings)
    {
        random = new Random();
        Settings = settings;
        NodeDataMap = new Dictionary<string, NodeSimilarityResult>();
    }

    public void UpdateSettings(UserSettings settings) => Settings = settings;

    public Graph BuildGraph(string name, List<NodeSimilarityResult> similarityResults)
    {
        NodeDataMap.Clear();

        var graph = new Graph(name)
        {
            LayoutAlgorithmSettings = new MdsLayoutSettings(),
        };

        var usedColors = new HashSet<SystemColor>();
        var addedEdges = new HashSet<Tuple<string, string>>();

        foreach (var similarityResult in similarityResults)
        {
            var currentNodeName = similarityResult.Node.Name;
            var node = AddNode(graph, currentNodeName, GetNewRandomColor(usedColors));
            NodeDataMap[node.Id] = similarityResult;

            foreach (var similarNode in similarityResult.SimilarNodes)
            {
                if (similarNode.SimilarityPercentage < Settings.MinSimilarityPercentage)
                    continue;

                var similarNodeName = similarNode.Node.Name;
                AddNode(graph, similarNodeName, GetNewRandomColor(usedColors));

                var edgeKey = Tuple.Create(currentNodeName, similarNodeName);
                var edgeKeyReversed = Tuple.Create(similarNodeName, currentNodeName);

                if (addedEdges.Contains(edgeKey) || addedEdges.Contains(edgeKeyReversed))
                    continue;

                AddEdge(graph, currentNodeName, similarNodeName);
                addedEdges.Add(edgeKey);
                addedEdges.Add(edgeKeyReversed);
            }
        }

        return graph;
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

    private SystemColor GetNewRandomColor(HashSet<SystemColor> usedColors)
    {
        SystemColor randColor;

        do
        {
            randColor = SystemColor.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        } while (usedColors.Contains(randColor));

        usedColors.Add(randColor);

        return randColor;
    }
}
