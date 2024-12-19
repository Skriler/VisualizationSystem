using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.DAL;
using VisualizationSystem.Services.Utilities.Clusterers;

namespace VisualizationSystem.Services.Utilities;

public class UserSettingsManager
{
    private readonly UserSettingsRepository userSettingsRepository;

    public UserSettings? UserSettings { get; private set; }

    public UserSettingsManager(UserSettingsRepository userSettingsRepository)
    {
        this.userSettingsRepository = userSettingsRepository;
    }

    public bool UseClustering() => UserSettings.UseClustering;

    public async Task LoadAsync(NodeTable table)
    {
        if (await userSettingsRepository.ExistsAsync(table.Name))
        {
            UserSettings = await userSettingsRepository.GetByTableNameAsync(table.Name);
        }
        else
        {
            UserSettings = InitializeNodeTableData(table, ClusterAlgorithm.Agglomerative);
            await userSettingsRepository.AddAsync(UserSettings);
        }
    }

    public async Task UpdateAsync(UserSettings userSettings)
    {
        await userSettingsRepository.UpdateAsync(userSettings);
        UserSettings = userSettings;
    }

    private UserSettings InitializeNodeTableData(NodeTable nodeTable, ClusterAlgorithm algorithm)
    {
        var userSettings = new UserSettings
        {
            NodeTable = nodeTable,
            AlgorithmSettings = new ClusterAlgorithmSettings()
        };

        userSettings.ParameterStates = nodeTable.ParameterTypes
            .Select(p => new ParameterState(p, userSettings))
            .ToList();

        userSettings.ResetCoreValues();

        return userSettings;
    }
}
