using VisualizationSystem.Models.Entities.Settings;

namespace VisualizationSystem.Services.Utilities.Settings;

public interface ISettingsObserver
{
    void Update(UserSettings settings);
}
