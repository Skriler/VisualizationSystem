using VisualizationSystem.Models.Storages;

namespace VisualizationSystem.UI.Forms;

public partial class SettingsForm : Form
{
    private static readonly string ValueLabelPrefix = "Value: ";

    private readonly ComparisonSettings settings;
    private int previousIndex = -1;

    public SettingsForm(ComparisonSettings comparisonSettings)
    {
        InitializeComponent();

        settings = comparisonSettings;

        nudMinSimilarityPercentage.Value = (decimal)settings.MinSimilarityPercentage;
        nudDeviationPercent.Value = (decimal)settings.DeviationPercent;

        InititalizeParameterStatesPanel(settings);
    }

    private void cmbNames_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (previousIndex >= 0)
            SaveParameterState(previousIndex);

        UpdateParameterStatesPanel();
    }

    private void btnSubmit_Click(object sender, EventArgs e)
    {
        settings.MinSimilarityPercentage = (float)nudMinSimilarityPercentage.Value;
        settings.DeviationPercent = (float)nudDeviationPercent.Value;
        SaveParameterState(previousIndex);

        DialogResult = DialogResult.OK;
        Close();
    }

    private void InititalizeParameterStatesPanel(ComparisonSettings settings)
    {
        cmbNames.Items.AddRange(
            settings.ParameterStates
            .Select(paramState => paramState.ParameterType.Name)
            .ToArray()
        );

        if (cmbNames.Items.Count <= 0)
            return;

        cmbNames.SelectedIndex = 0;
        UpdateParameterStatesPanel();
    }

    private void UpdateParameterStatesPanel()
    {
        var selectedName = cmbNames.SelectedItem?.ToString();
        var newSelectedParameterState = settings.ParameterStates
            .FirstOrDefault(p => p.ParameterType.Name == selectedName);

        if (newSelectedParameterState == null)
            return;

        nudWeight.Value = (decimal)newSelectedParameterState.Weight;
        chkbxIsActive.Checked = newSelectedParameterState.IsActive;
        previousIndex = cmbNames.SelectedIndex;
    }

    private void SaveParameterState(int index)
    {
        if (index < 0 || index >= settings.ParameterStates.Count)
            return;

        var parameterName = cmbNames.Items[index]?.ToString();
        var parameterState = settings.ParameterStates
            .FirstOrDefault(p => p.ParameterType.Name == parameterName);

        if (parameterState == null)
            return;

        parameterState.Weight = (float)nudWeight.Value;
        parameterState.IsActive = chkbxIsActive.Checked;
    }
}
