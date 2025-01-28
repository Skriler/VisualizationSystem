namespace VisualizationSystem.UI.Components.PanelControls;

public abstract class PanelControls
{
    public Panel Panel { get; }

    protected PanelControls(Panel panel)
    {
        Panel = panel;
    }
}
