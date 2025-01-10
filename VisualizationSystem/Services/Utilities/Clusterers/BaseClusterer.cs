using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Domain.Clusters;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Services.Utilities.Settings;
using VisualizationSystem.Services.Utilities.DistanceCalculators;

namespace VisualizationSystem.Services.Utilities.Clusterers;

public abstract class BaseClusterer : ISettingsObserver
{
    protected readonly DataNormalizer dataNormalizer;
    protected readonly MetricDistanceCalculator distanceCalculator;

    protected UserSettings settings;

    protected BaseClusterer(
        DataNormalizer dataNormalizer,
        MetricDistanceCalculator distanceCalculator,
        ISettingsSubject settingsSubject
        )
    {
        this.dataNormalizer = dataNormalizer;
        this.distanceCalculator = distanceCalculator;

        settingsSubject.Attach(this);
    }

    public abstract Task<List<Cluster>> ClusterAsync(NodeTable nodeTable);

    public void Update(UserSettings settings) => this.settings = settings;
}
