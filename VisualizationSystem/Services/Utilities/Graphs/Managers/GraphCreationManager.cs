﻿using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.Utilities.Comparers;
using VisualizationSystem.Services.Utilities.Factories;
using VisualizationSystem.Services.Utilities.Graphs.Builders;
using VisualizationSystem.Services.Utilities.Settings;

namespace VisualizationSystem.Services.Utilities.Graphs.Managers;

public class GraphCreationManager<TGraph> : ISettingsObserver
{
    protected readonly IGraphBuilder<TGraph> graphBuilder;
    protected readonly SimilarityComparer similarityComparer;
    protected readonly ClustererFactory clustererFactory;

    private UserSettings settings = default!;

    public GraphCreationManager(
        IGraphBuilder<TGraph> graphBuilder,
        SimilarityComparer similarityComparer,
        ClustererFactory clustererFactory,
        ISettingsSubject settingsSubject
        )
    {
        this.graphBuilder = graphBuilder;
        this.similarityComparer = similarityComparer;
        this.clustererFactory = clustererFactory;

        settingsSubject.Attach(this);
    }

    public void Update(UserSettings settings) => this.settings = settings;

    public async Task<TGraph> BuildGraphAsync(NodeTable nodeTable)
    {
        if (settings.UseClustering)
        {
            return await BuildClusteredGraphAsync(nodeTable);
        }

        return BuildGraph(nodeTable);
    }

    protected TGraph BuildGraph(NodeTable nodeTable)
    {
        var similarityResults = similarityComparer.CalculateSimilarNodes(nodeTable.NodeObjects);

        return graphBuilder.Build(nodeTable.Name, similarityResults);
    }

    protected async Task<TGraph> BuildClusteredGraphAsync(NodeTable nodeTable)
    {
        var clusterer = clustererFactory.Create(settings.AlgorithmSettings.SelectedAlgorithm);
        var clusters = await clusterer.ClusterAsync(nodeTable);

        if (settings.UseClusteredGraph)
        {
            var similarityResults = similarityComparer.CalculateSimilarNodes(nodeTable.NodeObjects);
            return graphBuilder.Build(nodeTable.Name, similarityResults, clusters);
        }

        return graphBuilder.Build(nodeTable.Name, nodeTable.NodeObjects, clusters);
    }
}
