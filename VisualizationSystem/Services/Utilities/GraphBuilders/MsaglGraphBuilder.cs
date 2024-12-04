using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.MDS;
using VisualizationSystem.Models.Storages;
using MsaglColor = Microsoft.Msagl.Drawing.Color;
using SystemColor = System.Drawing.Color;

namespace VisualizationSystem.Services.Utilities.GraphBuilders;

public class MsaglGraphBuilder : GraphBuilder<Graph>
{
    public override Graph Build(string name, List<NodeSimilarityResult> similarityResults, List<Cluster> clusters)
    {
        var graph = new Graph(name)
        {
            LayoutAlgorithmSettings = new MdsLayoutSettings(),
        };

        AddNodes(graph, similarityResults);
        AddEdges(graph, similarityResults);
        AddClusters(graph, clusters);

        return graph;
    }

    protected override void AddClusters(Graph graph, List<Cluster> clusters)
    {
        var rootSubgraph = new Subgraph("Main");

        foreach (var cluster in clusters)
        {
            var subgraph = new Subgraph(cluster.Id.ToString());
            subgraph.Attr.FillColor = MsaglColor.GreenYellow;
            subgraph.LabelText = $"Cluster: {cluster.Id.ToString()}";

            foreach (var nodeObject in cluster.Nodes)
            {
                var node = graph.FindNode(nodeObject.Name);

                if (node == null)
                    continue;

                subgraph.AddNode(node);
            }

            rootSubgraph.AddSubgraph(subgraph);
        }

        graph.RootSubgraph.AddSubgraph(rootSubgraph);
    }

    protected override void AddNode(Graph graph, string nodeName, SystemColor nodeColor, int edgesCount, int maxEdges)
    {
        var node = graph.FindNode(nodeName);

        if (node != null)
            return;

        node = new Node(nodeName)
        {
            LabelText = nodeName,
            Attr =
            {
                FillColor = new MsaglColor(nodeColor.R, nodeColor.G, nodeColor.B),
                //LabelMargin = (int)GetNodeSize(edgesCount, maxEdges)
            }
        };

        graph.AddNode(node);
    }

    protected override void AddEdge(Graph graph, string firstNodeName, string secondNodeName, float similarityPercentage)
    {
        var edge = graph.AddEdge(firstNodeName, secondNodeName);

        edge.Attr.ArrowheadAtSource = ArrowStyle.None;
        edge.Attr.ArrowheadAtTarget = ArrowStyle.None;

        //edge.Attr.Length = 1.0 / similarityPercentage;

        var edgeColor = CalculateEdgeColor(similarityPercentage, Settings.MinSimilarityPercentage);

        edge.Attr.Color = new MsaglColor(edgeColor.A, edgeColor.R, edgeColor.G, edgeColor.B);

        //edge.LabelText = $"{similarityPercentage:F1}%";
    }
}
