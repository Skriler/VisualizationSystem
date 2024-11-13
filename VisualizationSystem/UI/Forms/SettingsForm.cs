using VisualizationSystem.Models.Storages;

namespace VisualizationSystem.UI.Forms;

public partial class SettingsForm : Form
{
    private static readonly string ValueLabelPrefix = "Value: ";

    private readonly ComparisonSettings settings;

    public SettingsForm(ComparisonSettings comparisonSettings)
    {
        InitializeComponent();

        settings = comparisonSettings;

        var activeParametersCount = settings.GetActiveParameters().Count;

        trcbrMinMatchingParameters.Maximum = activeParametersCount;
        trcbrMinMatchingParameters.Value = 
            settings.MinMatchingParameters > activeParametersCount ?
            activeParametersCount : 
            settings.MinMatchingParameters;
        trcbrDeviationPercent.Value = (int)settings.DeviationPercent;

        UpdateLabelValue(lblMinMatchingParametersValue, trcbrMinMatchingParameters);
        UpdateLabelValue(lblDeviationPercentValue, trcbrDeviationPercent);
        InititalizeClbSelectedParameters(settings);
    }

    private void trcbrMinMatchingParameters_Scroll(object sender, EventArgs e)
    {
        UpdateLabelValue(lblMinMatchingParametersValue, trcbrMinMatchingParameters);
    }

    private void trcbrDeviationPercent_Scroll(object sender, EventArgs e)
    {
        UpdateLabelValue(lblDeviationPercentValue, trcbrDeviationPercent);
    }

    private void btnSubmit_Click(object sender, EventArgs e)
    {
        settings.MinMatchingParameters = trcbrMinMatchingParameters.Value;
        settings.DeviationPercent = trcbrDeviationPercent.Value;
        SaveParameterStatesToSettings();

        DialogResult = DialogResult.OK;
        Close();
    }

    private void UpdateLabelValue(Label label, TrackBar trackBar)
    {
        label.Text = ValueLabelPrefix + trackBar.Value.ToString();
    }

    private void InititalizeClbSelectedParameters(ComparisonSettings settings)
    {
        foreach (var paramState in settings.ParameterStates)
        {
            clbSelectedParams.Items.Add(
                paramState.ParameterType.Name,
                paramState.IsActive
            );
        }
    }

    private void SaveParameterStatesToSettings()
    {
        foreach (var item in clbSelectedParams.Items.Cast<string>())
        {
            var paramState = settings.ParameterStates
                .FirstOrDefault(p => p.ParameterType.Name == item);

            if (paramState == null)
                continue;

            paramState.IsActive = clbSelectedParams.CheckedItems.Contains(item);
        }
    }
}
