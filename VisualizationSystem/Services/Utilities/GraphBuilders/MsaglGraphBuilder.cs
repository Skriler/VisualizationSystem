using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Layout.MDS;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Storages.Results;
using Cluster = VisualizationSystem.Models.Storages.Clusters.Cluster;
using MsaglColor = Microsoft.Msagl.Drawing.Color;
using SystemColor = System.Drawing.Color;

namespace VisualizationSystem.Services.Utilities.GraphBuilders;

public class MsaglGraphBuilder : GraphBuilder<Graph>
{
    public MsaglGraphBuilder(GraphColorAssigner colorAssigner) 
        : base(colorAssigner)
    { }

    public override Graph Build(string name, List<NodeSimilarityResult> similarityResults)
    {
        var graph = new Graph(name)
        {
            LayoutAlgorithmSettings = new MdsLayoutSettings(),
        };

        AddNodes(graph, similarityResults);
        AddEdges(graph, similarityResults);

        return graph;
    }

    public override Graph Build(string name, List<NodeObject> nodes, List<Cluster> clusters)
    {
        var graph = new Graph(name)
        {
            LayoutAlgorithmSettings = new SugiyamaLayoutSettings(),
        };

        AddNodes(graph, nodes, clusters);
        //AddEdges(graph, similarityResults);
        AddClusters(graph, clusters);

        return graph;
    }

    protected override void AddClusters(Graph graph, List<Cluster> clusters)
    {
        NodeDataMap.Clear();

        foreach (var cluster in clusters)
        {
            var subgraph = new Subgraph(cluster.Id.ToString())
            {
                LabelText = $"Cluster: {cluster.Id}",
            };

            foreach (var node in cluster.Nodes)
            {
                var existingNode = graph.FindNode(node.Name);

                if (existingNode == null)
                    continue;

                subgraph.AddNode(existingNode);
            }

            graph.RootSubgraph.AddSubgraph(subgraph);
        }
    }

    protected override void AddNode(Graph graph, string nodeName, SystemColor nodeColor)
    {
        var node = new Node(nodeName)
        {
            LabelText = nodeName,
            Attr =
            {
                FillColor = new MsaglColor(nodeColor.R, nodeColor.G, nodeColor.B),
                //LabelMargin = (int)GetNodeSize(edgesCount, maxEdges) TODO
            }
        };

        graph.AddNode(node);
    }

    protected override void AddEdge(Graph graph, string firstNodeName, string secondNodeName, float similarityPercentage)
    {
        var edge = graph.AddEdge(firstNodeName, secondNodeName);

        edge.Attr.ArrowheadAtSource = ArrowStyle.None;
        edge.Attr.ArrowheadAtTarget = ArrowStyle.None;

        // TODO
        //edge.LabelText = $"{similarityPercentage:F1}%";
        //edge.Attr.Length = 1.0 / similarityPercentage;

        var edgeColor = colorAssigner.CalculateEdgeColor(similarityPercentage, Settings.MinSimilarityPercentage);
        edge.Attr.Color = new MsaglColor(edgeColor.A, edgeColor.R, edgeColor.G, edgeColor.B);
    }
}
