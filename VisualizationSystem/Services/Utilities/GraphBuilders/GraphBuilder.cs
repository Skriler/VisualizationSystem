﻿using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Storages.Clusters;
using VisualizationSystem.Models.Storages.Results;

namespace VisualizationSystem.Services.Utilities.GraphBuilders;

public abstract class GraphBuilder<TGraph> : IGraphBuilder<TGraph>
{
    protected readonly GraphColorAssigner colorAssigner;

    protected readonly Random random = new();
    protected readonly Dictionary<string, Color> nodeColors = new();

    public Dictionary<string, NodeSimilarityResult> NodeDataMap { get; } = new();
    public UserSettings Settings { get; set; } = new();

    protected GraphBuilder(GraphColorAssigner colorAssigner)
    {
        this.colorAssigner = colorAssigner;
    }

    public abstract TGraph Build(string name, List<NodeSimilarityResult> similarityResults);
    public abstract TGraph Build(string name, List<NodeObject> nodes, List<Cluster> clusters);

    protected virtual void AddNodes(TGraph graph, List<NodeSimilarityResult> similarityResults)
    {
        NodeDataMap.Clear();

        var minSimilarity = similarityResults.Min(result => result.SimilarNodes.Min(s => s.SimilarityPercentage));
        var maxSimilarity = similarityResults.Max(result => result.SimilarNodes.Max(s => s.SimilarityPercentage));

        foreach (var similarityResult in similarityResults)
        {
            var currentNodeName = similarityResult.Node.Name;

            var nodeColor = colorAssigner
                .GetNodeColorFromDensityWithSigmoid(similarityResult, similarityResults.Count, Settings.MinSimilarityPercentage);

            AddNode(
                graph, 
                currentNodeName, 
                nodeColor
                );
            
            NodeDataMap[currentNodeName] = similarityResult;
        }
    }

    protected virtual void AddNodes(TGraph graph, List<NodeObject> nodes, List<Cluster> clusters)
    {
        NodeDataMap.Clear();

        foreach (var node in nodes)
        {
            var currentNodeName = node.Name;

            var cluster = clusters.FirstOrDefault(c => c.Nodes.Contains(node));
            var nodeColor = cluster != null
                ? GetColorByName(cluster.Id.ToString())
                : GetColorByName(node.Name);

            AddNode(
                graph,
                currentNodeName,
                nodeColor
            );
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

    protected Color GetColorByName(string name)
    {
        if (nodeColors.TryGetValue(name, out Color nodeColor))
            return nodeColor;

        var newColor = GetNewRandomColor();
        nodeColors[name] = newColor;

        return newColor;
    }

    protected Color GetNewRandomColor()
    {
        Color randColor;

        do
        {
            randColor = Color.FromArgb(
                random.Next(128, 256),
                random.Next(128, 256),
                random.Next(128, 256)
            );
        } while (nodeColors.Values.Contains(randColor));

        return randColor;
    }

    protected double GetNodeSize(int connectionCount, int maxConnections)
    {
        return 1 + connectionCount / (double)maxConnections * 20;
    }

    protected abstract void AddClusters(TGraph graph, List<Cluster> clusters);
    protected abstract void AddNode(TGraph graph, string nodeName, Color nodeColor);
    protected abstract void AddEdge(TGraph graph, string firstNodeName, string secondNodeName, float similarityPercentage);
}
