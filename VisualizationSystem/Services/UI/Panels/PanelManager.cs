using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.UI.Components.PanelControls;

namespace VisualizationSystem.Services.UI.Panels;

public abstract class PanelManager<T> where T : PanelControls
{
    protected readonly UserSettings settings;

    public T Controls { get; }

    public bool IsVisible { get; set; } = true;

    protected PanelManager(UserSettings settings, T controls)
    {
        this.settings = settings;
        Controls = controls;
    }

    public abstract void Initialize();
    public abstract void UpdateContent();
    public abstract void SavePrevious();
    public abstract void Save();
}
