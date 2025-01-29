using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.Entities.Nodes;
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
        var analysisResult = new TableAnalysisResult(nodeTable);

        if (settings.UseNormalGraph || settings.UseClusteredGraph)
        {
            analysisResult.SimilarityResults =
                similarityComparer.CalculateSimilarNodes(analysisResult.Nodes);
        }

        if (settings.UseClustering)
        {
            var clusterer = clustererFactory.Create(settings.AlgorithmSettings.SelectedAlgorithm);
            analysisResult.Clusters = await clusterer.ClusterAsync(nodeTable);
        }

        return graphBuilder.Build(analysisResult);
    }
}
