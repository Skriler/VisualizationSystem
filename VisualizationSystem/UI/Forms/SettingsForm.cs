using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.UI.Panels;
using VisualizationSystem.Services.Utilities.Factories;

namespace VisualizationSystem.UI.Forms;

public partial class SettingsForm : Form
{
    private readonly UserSettings settings;
    private readonly UserSettingsFactory userSettingsFactory;

    private readonly ParameterStatesPanelManager parameterStatesPanel;
    private readonly ClusteringAlgorithmsPanelManager clusteringOptionsPanel;

    public SettingsForm(UserSettings comparisonSettings, UserSettingsFactory userSettingsFactory)
    {
        InitializeComponent();

        settings = comparisonSettings;
        this.userSettingsFactory = userSettingsFactory;

        parameterStatesPanel = new ParameterStatesPanelManager(settings, panelParameterStates, cmbNames, nudWeight, chkbxIsActive);
        clusteringOptionsPanel = new ClusteringAlgorithmsPanelManager(
            settings, panelClusteringOptions, cmbClusterAlgorithm, 
            nudFirstParameter, nudSecondParameter, lblFirstParameter, 
            lblSecondParameter, panelParameterStates.Location
            );

        InitializeMainControls();

        parameterStatesPanel.Initialize();
        clusteringOptionsPanel.Initialize();
    }

    private void chkbxUseClustering_CheckedChanged(object sender, EventArgs e)
    {
        panelClusteringOptions.Visible = chkbxUseClustering.Checked;
        panelParameterStates.Visible = !chkbxUseClustering.Checked;
    }

    private void cmbNames_SelectedValueChanged(object sender, EventArgs e)
    {
        if (cmbNames.SelectedItem == null)
            return;

        parameterStatesPanel.SavePrevious();
        parameterStatesPanel.UpdateContent();
    }

    private void cmbClusterAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbClusterAlgorithm.SelectedItem == null)
            return;

        clusteringOptionsPanel.SavePrevious();
        clusteringOptionsPanel.UpdateContent();
    }

    private void btnSubmit_Click(object sender, EventArgs e)
    {
        settings.MinSimilarityPercentage = (float)nudMinSimilarityPercentage.Value;
        settings.DeviationPercent = (float)nudDeviationPercent.Value;
        settings.UseClustering = chkbxUseClustering.Checked;

        parameterStatesPanel.SavePrevious();
        clusteringOptionsPanel.SavePrevious();

        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnSetDefaults_Click(object sender, EventArgs e)
    {
        settings.ResetToDefaults();
        InitializeMainControls();
        parameterStatesPanel.UpdateContent();
        clusteringOptionsPanel.UpdateContent();
    }

    private void InitializeMainControls()
    {
        nudMinSimilarityPercentage.Value = (decimal)settings.MinSimilarityPercentage;
        nudDeviationPercent.Value = (decimal)settings.DeviationPercent;
        chkbxUseClustering.Checked = settings.UseClustering;
    }
}
