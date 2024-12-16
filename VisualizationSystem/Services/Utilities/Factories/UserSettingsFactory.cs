using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.Utilities.Clusterers;

namespace VisualizationSystem.Services.Utilities.Factories;

public class UserSettingsFactory
{
    public UserSettings InitializeNodeTableData(NodeTable nodeTable, ClusterAlgorithm algorithm)
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
