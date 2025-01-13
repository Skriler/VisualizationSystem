using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Models.Domain.Plots;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.Utilities.DimensionReducers;
using VisualizationSystem.Services.Utilities.Factories;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Services.Utilities.Settings;

namespace VisualizationSystem.Services.Utilities.Plot;

public class PlotCreationManager : ISettingsObserver
{
    protected readonly ClustererFactory clustererFactory;
    protected readonly DataNormalizer dataNormalizer;
    protected readonly IDimensionReducer dimensionReducer;

    private UserSettings settings;

    public PlotCreationManager(
        ClustererFactory clustererFactory,
        DataNormalizer dataNormalizer,
        IDimensionReducer dimensionReducer,
        ISettingsSubject settingsSubject
        )
    {
        this.clustererFactory = clustererFactory;
        this.dataNormalizer = dataNormalizer;
        this.dimensionReducer = dimensionReducer;

        settingsSubject.Attach(this);
    }

    public void Update(UserSettings settings) => this.settings = settings;

    public async Task<ClusteredPlot> BuildClusteredPlotAsync(NodeTable nodeTable)
    {
        var clusterer = clustererFactory.CreateClusterer(settings.AlgorithmSettings.SelectedAlgorithm);
        var clusters = await clusterer.ClusterAsync(nodeTable);

        var plotData = dimensionReducer.ReduceDimension(clusters);

        return new ClusteredPlot(nodeTable.Name, plotData);
    }
}
