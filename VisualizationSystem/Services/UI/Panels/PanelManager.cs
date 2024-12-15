using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.UI.Forms;

namespace VisualizationSystem.Services.UI.Panels;

public abstract class PanelManager
{
    protected readonly UserSettings settings;
    protected readonly Panel panel;

    protected PanelManager(UserSettings settings, Panel panel)
    {
        this.panel = panel;
        this.settings = settings;
    }

    public abstract void Initialize();
    public abstract void UpdateContent();
    public abstract void SavePrevious();
}
