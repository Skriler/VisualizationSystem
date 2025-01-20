using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.DAL.Repositories;
using VisualizationSystem.Services.Utilities.Clusterers;
using VisualizationSystem.UI.Forms;

namespace VisualizationSystem.Services.Utilities.Settings;

public class UserSettingsManager : ISettingsSubject, ISettingsManager
{
    private readonly UserSettingsRepository settingsRepository;
    private readonly List<ISettingsObserver> settingsObservers = new();

    private UserSettings userSettings = new();

    public UserSettingsManager(UserSettingsRepository settingsRepository)
    {
        this.settingsRepository = settingsRepository;
    }

    public void Detach(ISettingsObserver observer) => settingsObservers.Remove(observer);

    public void Notify() => settingsObservers.ForEach(observer => observer.Update(userSettings));

    public bool UseClustering() => userSettings.UseClustering;

    public void Attach(ISettingsObserver observer)
    {
        settingsObservers.Add(observer);
        Notify();
    }

    public async Task LoadAsync(NodeTable table)
    {
        if (await settingsRepository.ExistsAsync(table.Name))
        {
            userSettings = await settingsRepository.GetByTableNameAsync(table.Name);
        }
        else
        {
            userSettings = InitializeNodeTableData(table, ClusterAlgorithm.KMeans);
            await settingsRepository.AddAsync(userSettings);
        }

        Notify();
    }

    public async Task<bool> TryOpenSettingsForm()
    {
        using var settingsForm = new SettingsForm(userSettings);

        if  (settingsForm.ShowDialog() == DialogResult.OK)
        {
            await settingsRepository.UpdateAsync(userSettings);
            Notify();

            return true;
        }
        
        return false;
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
