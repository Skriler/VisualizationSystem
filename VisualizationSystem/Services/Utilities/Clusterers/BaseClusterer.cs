using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Services.Utilities.Settings;
using VisualizationSystem.Services.Utilities.DistanceCalculators;
using VisualizationSystem.Services.Utilities.Factories;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public abstract class BaseClusterer : ISettingsObserver
{
    protected readonly DataNormalizer dataNormalizer;
    protected readonly IDistanceCalculator distanceCalculator;

    protected abstract ClusterAlgorithm Algorithm { get; }

    protected UserSettings settings = default!;

    protected BaseClusterer(
        DataNormalizer dataNormalizer,
        DistanceCalculatorFactory distanceCalculatorFactory,
        ISettingsSubject settingsSubject
        )
    {
        this.dataNormalizer = dataNormalizer;
        distanceCalculator = distanceCalculatorFactory.Create(Algorithm);

        settingsSubject.Attach(this);
    }

    public abstract Task<List<Cluster>> ClusterAsync(NodeTable nodeTable);

    public void Update(UserSettings settings) => this.settings = settings;
}
