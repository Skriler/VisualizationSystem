using VisualizationSystem.Models.Storages;

namespace VisualizationSystem.Forms;

public partial class SettingsForm : Form
{
    private static readonly string ValueLabelPrefix = "Value: ";

    private readonly ComparisonSettings settings;

    public SettingsForm(ComparisonSettings comparisonSettings)
    {
        InitializeComponent();

        settings = comparisonSettings;

        trcbrMinMatchingParameters.Value = settings.MinMatchingParameters;
        trcbrDeviationPercent.Value = (int)settings.DeviationPercent;

        UpdateLabelValue(lblMinMatchingParametersValue, trcbrMinMatchingParameters);
        UpdateLabelValue(lblDeviationPercentValue, trcbrDeviationPercent);
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

        DialogResult = DialogResult.OK;
        Close();
    }

    private void UpdateLabelValue(Label label, TrackBar trackBar)
    {
        label.Text = ValueLabelPrefix + trackBar.Value.ToString();
    }
}
