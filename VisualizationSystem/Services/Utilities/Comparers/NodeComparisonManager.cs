﻿using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.Utilities.Clusterers;
using VisualizationSystem.Services.Utilities.Factories;

namespace VisualizationSystem.Services.Utilities.Comparers;

public class NodeComparisonManager
{
    private readonly ClustererFactory clustererFactory;
    private readonly ICompare comparer;

    private BaseClusterer clusterer;

    public UserSettings Settings { get; private set; }

    public NodeComparisonManager(ClustererFactory clustererFactory, ICompare comparer)
    {
        this.clustererFactory = clustererFactory;
        this.comparer = comparer;
    }

    public void UpdateSettings(UserSettings settings)
    {
        Settings = settings;
        clusterer = clustererFactory.CreateClusterer(Settings.AlgorithmSettings.SelectedAlgorithm);
        clusterer.UpdateSettings(settings.AlgorithmSettings);
    }

    public List<NodeSimilarityResult> CalculateSimilarNodes(List<NodeObject> nodes)
    {
        var similarityResultsDict = nodes
            .ToDictionary(node => node, node => new NodeSimilarityResult(node));

        for (int i = 0; i < nodes.Count; ++i)
        {
            var firstNode = nodes[i];

            for (int j = i + 1; j < nodes.Count; ++j)
            {
                var secondNode = nodes[j];

                var similarityPercentage = GetSimilarityPercentage(firstNode, secondNode);

                similarityResultsDict[firstNode]
                    .SimilarNodes
                    .Add(new SimilarNode(secondNode, similarityPercentage));

                similarityResultsDict[secondNode]
                    .SimilarNodes
                    .Add(new SimilarNode(firstNode, similarityPercentage));
            }
        }

        var similarityResults = similarityResultsDict.Values.ToList();

        similarityResults.ForEach(sr =>
        {
            sr.SimilarNodesAboveThreshold = sr.SimilarNodes
                .Count(sn => sn.SimilarityPercentage > Settings.MinSimilarityPercentage);
        });

        return similarityResults;
    }

    public async Task<List<Cluster>> CalculateClustersAsync(NodeTable nodeTable)
    {
        if (nodeTable.NodeObjects.Count <= 0)
            throw new InvalidOperationException("Node analysis must be performed first before calculating clusters.");

        return await clusterer.ClusterAsync(nodeTable);
    }

    private float GetSimilarityPercentage(NodeObject firstNode, NodeObject secondNode)
    {
        if (firstNode.Parameters.Count != secondNode.Parameters.Count)
            throw new InvalidOperationException("NodeObjects must have the same number of parameters");

        float totalMatchedWeight = 0;
        float totalActiveWeight = 0;

        var activeParameterStates = Settings.GetActiveParameters();
        var firstNodeParameters = firstNode.Parameters.ToDictionary(p => p.ParameterType);
        var secondNodeParameters = secondNode.Parameters.ToDictionary(p => p.ParameterType);

        foreach (var parameterState in activeParameterStates)
        {
            totalActiveWeight += parameterState.Weight;

            firstNodeParameters.TryGetValue(parameterState.ParameterType, out var firstParameter);
            secondNodeParameters.TryGetValue(parameterState.ParameterType, out var secondParameter);

            if (firstParameter == null || secondParameter == null)
                continue;

            if (firstParameter.ParameterType.Name != secondParameter.ParameterType.Name)
                continue;

            if (!comparer.Compare(firstParameter.Value, secondParameter.Value, Settings.DeviationPercent))
                continue;

            totalMatchedWeight += parameterState.Weight;
        }

        return totalActiveWeight > 0 ? totalMatchedWeight / totalActiveWeight * 100 : 0;

    }
}
