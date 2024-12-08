﻿using VisualizationSystem.Models.Entities;
using VisualizationSystem.Services.Utilities.Clusterers;

namespace VisualizationSystem.UI.Forms;

public partial class SettingsForm : Form
{
    private readonly UserSettings settings;
    private int previousIndex = -1;

    public SettingsForm(UserSettings comparisonSettings)
    {
        InitializeComponent();

        settings = comparisonSettings;

        InitializeMainControls();
        InitializeParameterStatesPanel();
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
        settings.UseClustering = chkbxUseClustering.Checked;

        var selectedAlgorithm = (ClusterAlgorithm)Enum.Parse(
            typeof(ClusterAlgorithm), 
            cmbClusterAlgorithm.SelectedItem.ToString()
            );
        settings.ClusterAlgorithm = selectedAlgorithm;

        SaveParameterState(previousIndex);

        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnSetDefaults_Click(object sender, EventArgs e)
    {
        settings.ResetToDefaults();
        InitializeMainControls();
        UpdateParameterStatesPanel();
    }

    private void InitializeMainControls()
    {
        nudMinSimilarityPercentage.Value = (decimal)settings.MinSimilarityPercentage;
        nudDeviationPercent.Value = (decimal)settings.DeviationPercent;
        chkbxUseClustering.Checked = settings.UseClustering;

        var clusterAlgorithms = Enum.GetNames(typeof(ClusterAlgorithm));
        cmbClusterAlgorithm.Items.AddRange(clusterAlgorithms);
        cmbClusterAlgorithm.SelectedItem = settings.ClusterAlgorithm.ToString();
    }

    private void InitializeParameterStatesPanel()
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
