using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Entities.Settings.AlgorithmSettings;
using VisualizationSystem.Services.DAL;
using VisualizationSystem.Services.Utilities.Clusterers;

namespace VisualizationSystem.Services.Utilities.Factories;

public class UserSettingsFactory
{
    private readonly ClusterAlgorithmSettingsFactory algorithmSettingsFactory;
    private readonly UserSettingsRepository userSettingsRepository;

    public UserSettingsFactory(ClusterAlgorithmSettingsFactory algorithmSettingsFactory, UserSettingsRepository userSettingsRepository)
    {
        this.algorithmSettingsFactory = algorithmSettingsFactory;
        this.userSettingsRepository = userSettingsRepository;
    }

    public UserSettings InitializeNodeTableData(NodeTable nodeTable, ClusterAlgorithm algorithm)
    {
        var userSettings = new UserSettings
        {
            NodeTable = nodeTable,
            AlgorithmSettings = algorithmSettingsFactory.CreateAlgorithmSettings(algorithm)
        };

        userSettings.ParameterStates = nodeTable.ParameterTypes
            .Select(p => new ParameterState(p, userSettings))
            .ToList();

        userSettings.ResetCoreValues();

        return userSettings;
    }

    public async Task ChangeAlgorithmSettingsAsync(UserSettings userSettings, ClusterAlgorithm newAlgorithm)
    {
        if (userSettings.AlgorithmSettings.GetType() != GetTypeForAlgorithm(newAlgorithm))
        {
            userSettings.AlgorithmSettings = await GetAlgorithmSettingsAsync(newAlgorithm);
        }

        ClusterAlgorithmSettings newSettings = newAlgorithm switch
        {
            ClusterAlgorithm.Agglomerative => new AgglomerativeSettings(),
            ClusterAlgorithm.KMeans => new KMeansSettings(),
            ClusterAlgorithm.DBSCAN => new DBSCANSettings(),
            _ => throw new ArgumentOutOfRangeException(nameof(newAlgorithm), "Unknown algorithm type.")
        };

        userSettings.AlgorithmSettings = newSettings;
        userSettings.AlgorithmSettings.SelectedAlgorithm = newAlgorithm;
    }

    private Type GetTypeForAlgorithm(ClusterAlgorithm algorithm)
    {
        return algorithm switch
        {
            ClusterAlgorithm.Agglomerative => typeof(AgglomerativeSettings),
            ClusterAlgorithm.KMeans => typeof(KMeansSettings),
            ClusterAlgorithm.DBSCAN => typeof(DBSCANSettings),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), "Unknown algorithm type.")
        };
    }

    private async Task<ClusterAlgorithmSettings> GetAlgorithmSettingsAsync(ClusterAlgorithm algorithm)
    {
        return algorithm switch
        {
            ClusterAlgorithm.Agglomerative => await db.AgglomerativeSettings.FirstOrDefaultAsync(),
            ClusterAlgorithm.KMeans => await db.KMeansSettings.FirstOrDefaultAsync(),
            ClusterAlgorithm.DBSCAN => await db.DBSCANSettings.FirstOrDefaultAsync(),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), "Unknown algorithm type.")
        };
    }
}
