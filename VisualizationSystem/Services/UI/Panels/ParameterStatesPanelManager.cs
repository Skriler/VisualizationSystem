using VisualizationSystem.Models.Domain.Settings;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.UI.Components.PanelControls;

namespace VisualizationSystem.Services.UI.Panels;

public class ParameterStatesPanelManager : PanelManager<ParameterStatePanelControls>
{
    private List<ParameterStateData> parametersList = default!;
    private string previousName = default!;

    public ParameterStatesPanelManager(
        UserSettings settings,
        ParameterStatePanelControls controls
        )
        : base(settings, controls)
    { }

    public override void Initialize()
    {
        previousName = string.Empty;
        parametersList = settings.ParameterStates
            .ConvertAll(p => new ParameterStateData(p));

        Controls.CmbNames.Items.Clear();
        Controls.CmbNames.Items.AddRange(
            parametersList
                .Select(p => p.Name)
                .ToArray()
        );

        if (Controls.CmbNames.Items.Count <= 0)
            return;

        Controls.CmbNames.SelectedIndex = 0;

        UpdateContent();
    }

    public override void UpdateContent()
    {
        if (Controls.CmbNames.SelectedIndex < 0)
            return;

        var selectedName = Controls.CmbNames.SelectedItem?.ToString();
        var selectedParameterState = parametersList
            .First(p => p.Name == selectedName);

        Controls.NudWeight.Value = (decimal)selectedParameterState.Weight;
        Controls.ChkbxIsActive.Checked = selectedParameterState.IsActive;

        previousName = selectedName;
    }

    public override void SavePrevious()
    {
        var parameterState = parametersList
            .FirstOrDefault(p => p.Name == previousName);

        if (parameterState == null)
            return;

        parameterState.Weight = (float)Controls.NudWeight.Value;
        parameterState.IsActive = Controls.ChkbxIsActive.Checked;
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
