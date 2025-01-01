using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Services.Utilities.Settings;

public interface ISettingsManager
{
    bool UseClustering();

    Task LoadAsync(NodeTable table);

    Task<bool> TryOpenSettingsForm();
}
