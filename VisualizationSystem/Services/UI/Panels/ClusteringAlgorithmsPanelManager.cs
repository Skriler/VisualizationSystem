using System.Windows.Forms;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Entities.Settings.AlgorithmSettings;
using VisualizationSystem.Models.Storages.Configs;
using VisualizationSystem.Services.Utilities.Clusterers;

namespace VisualizationSystem.Services.UI.Panels;

public class ClusteringAlgorithmsPanelManager : PanelManager
{
    private readonly ComboBox cmbClusterAlgorithm;
    private readonly NumericUpDown nudFirstParameter;
    private readonly NumericUpDown nudSecondParameter;
    private readonly Label lblFirstParameter;
    private readonly Label lblSecondParameter;

    private int previousAlgorithmIndex = -1;

    private Array algorithms;

    public ClusteringAlgorithmsPanelManager(
        UserSettings settings, Panel panel, ComboBox cmbClusterAlgorithm, 
        NumericUpDown nudFirstParameter, NumericUpDown nudSecondParameter, 
        Label lblFirstParameter, Label lblSecondParameter, Point panelLocation
        )
        : base(settings, panel)
    {
        this.cmbClusterAlgorithm = cmbClusterAlgorithm;
        this.nudFirstParameter = nudFirstParameter;
        this.nudSecondParameter = nudSecondParameter;
        this.lblFirstParameter = lblFirstParameter;
        this.lblSecondParameter = lblSecondParameter;

        panel.Location = panelLocation;
    }

    public override void Initialize()
    {
        panel.Visible = settings.UseClustering;

        var clusterAlgorithms = Enum.GetNames(typeof(ClusterAlgorithm));
        algorithms = Enum.GetValues(typeof(ClusterAlgorithm));

        cmbClusterAlgorithm.Items.Clear();
        cmbClusterAlgorithm.Items.AddRange(clusterAlgorithms);

        if (cmbClusterAlgorithm.Items.Count <= 0)
            return;

        cmbClusterAlgorithm.SelectedItem = settings.AlgorithmSettings.SelectedAlgorithm.ToString();

        UpdateContent();
    }


    public override void UpdateContent()
    {
        if(cmbClusterAlgorithm.SelectedIndex < 0)
            return;

        var algorithm = (ClusterAlgorithm)algorithms.GetValue(cmbClusterAlgorithm.SelectedIndex);

        Action algorithmSetupAction = algorithm switch
        {
            ClusterAlgorithm.Agglomerative => UpdateClusteringOptionsForAgglomerative,
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
            ClusterAlgorithm.Agglomerative => SaveAgglomerativeSettings,
            ClusterAlgorithm.KMeans => SaveKMeansSettings,
            ClusterAlgorithm.DBSCAN => SaveDBSCANSettings,
            _ => throw new ArgumentOutOfRangeException(nameof(previousAlgorithm), "Unknown clustering algorithm selected")
        };

        algorithmSetupAction();
    }

    private void UpdateClusteringOptionsForAgglomerative()
    {
        if (settings.AlgorithmSettings is not AgglomerativeSettings algorithmSettings)
            return;

        UpdateClusteringOptions(AlgorithmParameterConfigs.AgglomerativeConfig);

        nudFirstParameter.Value = (decimal)algorithmSettings.Threshold;
    }

    private void UpdateClusteringOptionsForKMeans()
    {
        if (settings.AlgorithmSettings is not KMeansSettings algorithmSettings)
            return;

        UpdateClusteringOptions(AlgorithmParameterConfigs.KMeansConfig);

        nudFirstParameter.Value = algorithmSettings.NumberOfClusters;
        nudSecondParameter.Value = algorithmSettings.MaxIterations;
    }

    private void UpdateClusteringOptionsForDBSCAN()
    {
        if (settings.AlgorithmSettings is not DBSCANSettings algorithmSettings)
            return;

        UpdateClusteringOptions(AlgorithmParameterConfigs.DBSCANConfig);

        nudFirstParameter.Value = (decimal)algorithmSettings.Epsilon;
        nudSecondParameter.Value = algorithmSettings.MinPoints;
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
    }

    private void SaveAgglomerativeSettings()
    {
        if (settings.AlgorithmSettings is not AgglomerativeSettings algorithmSettings)
            return;

        algorithmSettings.Threshold = (float)nudFirstParameter.Value;
    }

    private void SaveKMeansSettings()
    {
        if (settings.AlgorithmSettings is not KMeansSettings algorithmSettings)
            return;

        algorithmSettings.NumberOfClusters = (int)nudFirstParameter.Value;
        algorithmSettings.MaxIterations = (int)nudSecondParameter.Value;
    }

    private void SaveDBSCANSettings()
    {
        if (settings.AlgorithmSettings is not DBSCANSettings algorithmSettings)
            return;

        algorithmSettings.Epsilon = (float)nudFirstParameter.Value;
        algorithmSettings.MinPoints = (int)nudSecondParameter.Value;
    }
}
