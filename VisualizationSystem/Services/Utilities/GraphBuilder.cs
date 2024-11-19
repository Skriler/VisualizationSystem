using Microsoft.Msagl.Drawing;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using MsaglColor = Microsoft.Msagl.Drawing.Color;
using SystemColor = System.Drawing.Color;

namespace VisualizationSystem.Services.Utilities;

public class GraphBuilder
{
    private readonly Random random;

    private UserSettings settings;

    public GraphBuilder(UserSettings userSettings)
    {
        random = new Random();

        settings = userSettings;
    }

    public void UpdateSettings(UserSettings comparisonSettings) => settings = comparisonSettings;

    public Graph BuildGraph(List<NodeSimilarityResult> comparisonResults, NodeTable table)
    {
        var graph = new Graph(table.Name);
        var usedColors = new HashSet<SystemColor>();
        var addedEdges = new HashSet<Tuple<string, string>>();

        foreach (var similarityResult in comparisonResults)
        {
            var currentNodeName = similarityResult.Node.Name;
            AddNode(graph, currentNodeName, GetNewRandomColor(usedColors));

            foreach (var similarNode in similarityResult.SimilarNodes)
            {
                if (similarNode.SimilarityPercentage < settings.MinSimilarityPercentage)
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

    private bool AddNode(Graph graph, string nodeName, SystemColor nodeColor)
    {
        var node = graph.FindNode(nodeName);

        if (node != null)
            return false;

        node = graph.AddNode(nodeName);
        node.LabelText = nodeName;
        node.Attr.FillColor = new MsaglColor(nodeColor.R, nodeColor.G, nodeColor.B);

        return true;
    }

    private bool AddEdge(Graph graph, string firstNodeName, string secondNodeName)
    {
        var edge = graph.AddEdge(firstNodeName, secondNodeName);

        edge.Attr.ArrowheadAtSource = ArrowStyle.None;
        edge.Attr.ArrowheadAtTarget = ArrowStyle.None;

        return true;
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
