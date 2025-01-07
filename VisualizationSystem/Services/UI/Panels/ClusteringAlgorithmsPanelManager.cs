using VisualizationSystem.Models.Configs;
using VisualizationSystem.Models.Domain.Settings;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.Utilities.Clusterers;

namespace VisualizationSystem.Services.UI.Panels;

public class ClusteringAlgorithmsPanelManager : PanelManager
{
    private readonly ComboBox cmbClusterAlgorithm;
    private readonly NumericUpDown nudFirstParameter;
    private readonly NumericUpDown nudSecondParameter;
    private readonly Label lblFirstParameter;
    private readonly Label lblSecondParameter;

    private ClusterAlgorithmSettingsData settingsData;
    private int previousAlgorithmIndex;

    private Array algorithms;

    public ClusteringAlgorithmsPanelManager(
        UserSettings settings, Panel panel, ComboBox cmbClusterAlgorithm,
        NumericUpDown nudFirstParameter, NumericUpDown nudSecondParameter,
        Label lblFirstParameter, Label lblSecondParameter
        )
        : base(settings, panel)
    {
        this.cmbClusterAlgorithm = cmbClusterAlgorithm;
        this.nudFirstParameter = nudFirstParameter;
        this.nudSecondParameter = nudSecondParameter;
        this.lblFirstParameter = lblFirstParameter;
        this.lblSecondParameter = lblSecondParameter;

        algorithms = Enum.GetValues(typeof(ClusterAlgorithm));
    }

    public override void Initialize()
    {
        previousAlgorithmIndex = -1;
        settingsData = new ClusterAlgorithmSettingsData(settings.AlgorithmSettings);

        panel.Visible = settings.UseClustering;

        cmbClusterAlgorithm.Items.Clear();
        cmbClusterAlgorithm.Items.AddRange(Enum.GetNames(typeof(ClusterAlgorithm)));

        if (cmbClusterAlgorithm.Items.Count <= 0)
            return;

        cmbClusterAlgorithm.SelectedItem = settingsData.SelectedAlgorithm.ToString();

        UpdateContent();
    }


    public override void UpdateContent()
    {
        if(cmbClusterAlgorithm.SelectedIndex < 0)
            return;

        var algorithm = (ClusterAlgorithm)algorithms.GetValue(cmbClusterAlgorithm.SelectedIndex);
        settingsData.SelectedAlgorithm = algorithm;

        Action algorithmSetupAction = algorithm switch
        {
            ClusterAlgorithm.HierarchicalAgglomerative => UpdateClusteringOptionsForAgglomerative,
            ClusterAlgorithm.KMeans => UpdateClusteringOptionsForKMeans,
            ClusterAlgorithm.DBSCAN => UpdateClusteringOptionsForDBSCAN,
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), "Unknown clustering algorithm selected")
        };

        algorithmSetupAction();
        previousAlgorithmIndex = cmbClusterAlgorithm.SelectedIndex;
    }

    public override void SavePrevious()
    {
        if (previousAlgorithmIndex < 0)
            return;

        var previousAlgorithm = (ClusterAlgorithm)algorithms.GetValue(previousAlgorithmIndex);

        Action algorithmSetupAction = previousAlgorithm switch
        {
            ClusterAlgorithm.HierarchicalAgglomerative => SaveAgglomerativeSettings,
            ClusterAlgorithm.KMeans => SaveKMeansSettings,
            ClusterAlgorithm.DBSCAN => SaveDBSCANSettings,
            _ => throw new ArgumentOutOfRangeException(nameof(previousAlgorithm), "Unknown clustering algorithm selected")
        };

        algorithmSetupAction();
    }

    public override void Save()
    {
        SavePrevious();

        settings.AlgorithmSettings.SetData(settingsData);
    }

    private void UpdateClusteringOptionsForAgglomerative()
    {
        UpdateClusteringOptions(AlgorithmParameterConfigs.AgglomerativeConfig);

        nudFirstParameter.Value = (decimal)settingsData.Threshold;
    }

    private void UpdateClusteringOptionsForKMeans()
    {
        UpdateClusteringOptions(AlgorithmParameterConfigs.KMeansConfig);

        nudFirstParameter.Value = settingsData.NumberOfClusters;
        nudSecondParameter.Value = settingsData.MaxIterations;
    }

    private void UpdateClusteringOptionsForDBSCAN()
    {
        UpdateClusteringOptions(AlgorithmParameterConfigs.DBSCANConfig);

        nudFirstParameter.Value = (decimal)settingsData.Epsilon;
        nudSecondParameter.Value = settingsData.MinPoints;
    }

    private void UpdateClusteringOptions(AlgorithmParameterConfig config)
    {
        lblFirstParameter.Text = config.FirstParameterLabel;
        lblSecondParameter.Text = config.SecondParameterLabel;

        nudFirstParameter.Minimum = config.FirstParameterMin;
        nudFirstParameter.Maximum = config.FirstParameterMax;

        nudSecondParameter.Minimum = config.SecondParameterMin;
        nudSecondParameter.Maximum = config.SecondParameterMax;

        lblSecondParameter.Visible = config.HasSecondParameter;
        nudSecondParameter.Visible = config.HasSecondParameter;

        ConfigureNumericUpDown(nudFirstParameter, config.FirstParameterDecimalPlaces);
        ConfigureNumericUpDown(nudSecondParameter, config.SecondParameterDecimalPlaces);
    }

    private void ConfigureNumericUpDown(NumericUpDown nud, int decimalPlaces)
    {
        nud.DecimalPlaces = decimalPlaces;
        nud.Increment = (decimal)Math.Pow(10, -decimalPlaces);
    }

    private void SaveAgglomerativeSettings()
    {
        settingsData.Threshold = (float)nudFirstParameter.Value;
    }

    private void SaveKMeansSettings()
    {
        settingsData.NumberOfClusters = (int)nudFirstParameter.Value;
        settingsData.MaxIterations = (int)nudSecondParameter.Value;
    }

    private void SaveDBSCANSettings()
    {
        settingsData.Epsilon = (float)nudFirstParameter.Value;
        settingsData.MinPoints = (int)nudSecondParameter.Value;
    }
}
