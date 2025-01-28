
namespace VisualizationSystem.UI.Components.PanelControls;

public class ParameterStatePanelControls : PanelControls
{
    public ComboBox CmbNames { get; }

    public NumericUpDown NudWeight { get; }

    public CheckBox ChkbxIsActive { get; }

    public ParameterStatePanelControls(
        Panel panel,
        ComboBox cmbNames,
        NumericUpDown nudWeight,
        CheckBox chkbxIsActive
        )
        : base(panel)
    {
        CmbNames = cmbNames;
        NudWeight = nudWeight;
        ChkbxIsActive = chkbxIsActive;
    }
}
