using Microsoft.Msagl.Drawing;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;

using MsaglColor = Microsoft.Msagl.Drawing.Color;
using SystemColor = System.Drawing.Color;

namespace VisualizationSystem.SL;

public class GraphBuilder
{
    private readonly Random random;

    private ComparisonSettings settings;

    public GraphBuilder(ComparisonSettings comparisonSettings)
    {
        random = new Random();

        settings = comparisonSettings;
    }

    public void UpdateSettings(ComparisonSettings comparisonSettings) => settings = comparisonSettings;

    public Graph BuildGraph(List<NodeComparisonResult> comparisonResults, NodeTable table)
    {
        Graph graph = new Graph(table.Name);
        HashSet<SystemColor> usedColors = new HashSet<SystemColor>();

        string firstNodeName;
        string secondNodeName;
        foreach (var comparisonResult in comparisonResults)
        {
            firstNodeName = comparisonResult.FirstNode.Name;
            secondNodeName = comparisonResult.SecondNode.Name;

            AddNode(graph, firstNodeName, GetNewRandomColor(usedColors));
            AddNode(graph, secondNodeName, GetNewRandomColor(usedColors));

            if (comparisonResult.SimilarityPercentage < settings.MinSimilarityPercentage)
                continue;

            AddEdge(graph, firstNodeName, secondNodeName);
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
