using VisualizationSystem.Models.Entities.Settings;

namespace VisualizationSystem.Services.UI.Panels;

public class ParameterStatesPanelManager : PanelManager
{
    private readonly ComboBox cmbNames;
    private readonly NumericUpDown nudWeight;
    private readonly CheckBox chkbxIsActive;

    private string previousName;

    public ParameterStatesPanelManager(
        UserSettings settings, Panel panel, ComboBox cmbNames, 
        NumericUpDown nudWeight, CheckBox chkbxIsActive
        )
        : base(settings, panel)
    {
        this.cmbNames = cmbNames;
        this.nudWeight = nudWeight;
        this.chkbxIsActive = chkbxIsActive;
    }

    public override void Initialize()
    {
        panel.Visible = !settings.UseClustering;

        cmbNames.Items.Clear();
        cmbNames.Items.AddRange(
            settings.ParameterStates
                .Select(paramState => paramState.ParameterType.Name)
                .ToArray()
        );

        if (cmbNames.Items.Count <= 0)
            return;

        cmbNames.SelectedIndex = 0;

        UpdateContent();
    }

    public override void UpdateContent()
    {
        if (cmbNames.SelectedIndex < 0)
            return;

        var selectedName = cmbNames.SelectedItem?.ToString();
        var newSelectedParameterState = settings.ParameterStates
            .FirstOrDefault(p => p.ParameterType.Name == selectedName);

        if (newSelectedParameterState == null)
            return;

        nudWeight.Value = (decimal)newSelectedParameterState.Weight;
        chkbxIsActive.Checked = newSelectedParameterState.IsActive;

        previousName = selectedName;
    }

    public override void SavePrevious()
    {
        var parameterState = settings.ParameterStates
            .FirstOrDefault(p => p.ParameterType.Name == previousName);

        if (parameterState == null)
            return;

        parameterState.Weight = (float)nudWeight.Value;
        parameterState.IsActive = chkbxIsActive.Checked;
    }
}
