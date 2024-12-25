using VisualizationSystem.Models.Domain.Settings;
using VisualizationSystem.Models.Entities.Settings;

namespace VisualizationSystem.Services.UI.Panels;

public class ParameterStatesPanelManager : PanelManager
{
    private readonly ComboBox cmbNames;
    private readonly NumericUpDown nudWeight;
    private readonly CheckBox chkbxIsActive;

    private List<ParameterStateData> parametersList;
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
        previousName = string.Empty;
        parametersList = settings.ParameterStates
            .ConvertAll(p => new ParameterStateData(p));

        panel.Visible = !settings.UseClustering;

        cmbNames.Items.Clear();
        cmbNames.Items.AddRange(
            parametersList
                .Select(p => p.Name)
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
        var selectedParameterState = parametersList
            .FirstOrDefault(p => p.Name == selectedName);

        nudWeight.Value = (decimal)selectedParameterState.Weight;
        chkbxIsActive.Checked = selectedParameterState.IsActive;

        previousName = selectedName;
    }

    public override void SavePrevious()
    {
        var parameterState = parametersList
            .FirstOrDefault(p => p.Name == previousName);

        if (parameterState == null)
            return;

        parameterState.Weight = (float)nudWeight.Value;
        parameterState.IsActive = chkbxIsActive.Checked;
    }

    public override void Save()
    {
        SavePrevious();

        foreach (var parameter in parametersList)
        {
            var parameterState = settings.ParameterStates
                .FirstOrDefault(p => p.ParameterType.Name == parameter.Name);

            parameterState?.SetData(parameter);
        }
    }
}
