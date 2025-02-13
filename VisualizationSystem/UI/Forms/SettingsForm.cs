﻿using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.UI.Panels;
using VisualizationSystem.UI.Components.PanelControls;

namespace VisualizationSystem.UI.Forms;

public partial class SettingsForm : Form
{
    private readonly UserSettings settings;

    private readonly PanelManager<ParameterStatePanelControls> parameterStatesPanel;
    private readonly PanelManager<ClusteringAlgorithmPanelControls> clusteringOptionsPanel;

    private readonly Size startFormSize;
    private readonly Dictionary<Button, Point> startButtonPositions;

    public SettingsForm(UserSettings settings)
    {
        InitializeComponent();

        this.settings = settings;
        startFormSize = Size;
        startButtonPositions = new Dictionary<Button, Point>
        {
            { btnSubmit, btnSubmit.Location },
            { btnSetDefaults, btnSetDefaults.Location }
        };

        parameterStatesPanel = CreateParameterStatePanelManagers();
        clusteringOptionsPanel = CreateClusteringAlgorithmsPanelManagers();

        InitializeMainControls();
        parameterStatesPanel.Initialize();
        clusteringOptionsPanel.Initialize();
        UpdateFormLayout(chkbxUseClustering.Checked);
    }

    private void chkbxUseClustering_CheckedChanged(object sender, EventArgs e)
    {
        UpdateFormLayout(chkbxUseClustering.Checked);
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
        settings.UseClusteredGraph = chkbxWithEdges.Checked;

        parameterStatesPanel.Save();
        clusteringOptionsPanel.Save();

        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnSetDefaults_Click(object sender, EventArgs e)
    {
        settings.ResetToDefaults();
        InitializeMainControls();
        parameterStatesPanel.Initialize();
        clusteringOptionsPanel.Initialize();
    }

    private ParameterStatesPanelManager CreateParameterStatePanelManagers()
    {
        var parameterControls = new ParameterStatePanelControls(
            panelParameterStates,
            cmbNames,
            nudWeight,
            chkbxIsActive
        );

        return new ParameterStatesPanelManager(settings, parameterControls);
    }

    private ClusteringAlgorithmsPanelManager CreateClusteringAlgorithmsPanelManagers()
    {
        var clusteringControls = new ClusteringAlgorithmPanelControls(
            panelClusteringOptions,
            cmbClusterAlgorithm,
            nudFirstParameter,
            nudSecondParameter,
            lblFirstParameter,
            lblSecondParameter
        );

        return new ClusteringAlgorithmsPanelManager(settings, clusteringControls)
        {
            IsVisible = settings.UseClustering
        };
    }

    private void InitializeMainControls()
    {
        nudMinSimilarityPercentage.Value = (decimal)settings.MinSimilarityPercentage;
        nudDeviationPercent.Value = (decimal)settings.DeviationPercent;
        chkbxUseClustering.Checked = settings.UseClustering;
    }

    private void UpdateFormLayout(bool isClusteringEnabled)
    {
        panelClusteringOptions.Visible = isClusteringEnabled;
        chkbxWithEdges.Visible = isClusteringEnabled;
        chkbxWithEdges.Checked = settings.UseClusteredGraph;
        UpdateButtonPositions(isClusteringEnabled);
        UpdateFormSize(isClusteringEnabled);
    }

    private void UpdateButtonPositions(bool isClusteringEnabled)
    {
        foreach (var button in startButtonPositions)
        {
            var targetY = isClusteringEnabled ? button.Value.Y : panelClusteringOptions.Location.Y;

            button.Key.Location = new Point(button.Key.Location.X, targetY);
        }
    }

    private void UpdateFormSize(bool isClusteringEnabled)
    {
        Size = new Size(
            Width,
            startFormSize.Height - (isClusteringEnabled ? 0 : panelClusteringOptions.Height)
        );
    }
}
