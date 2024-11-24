namespace VisualizationSystem.Models.Storages;

public enum TabControlType
{
    DataGridView,
    GViewer,
}

public class TabPageInfo
{
    public TabPage Page { get; private set; }
    public TabControlType ControlType { get; private set; }
    public string Name { get; private set; }

    public TabPageInfo(TabPage tabPage, TabControlType controlType, string name)
    {
        Page = tabPage;
        ControlType = controlType;
        Name = name;
    }
}
