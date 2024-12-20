using VisualizationSystem.Models.Entities.Settings;
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

        settings.AlgorithmSettings.SelectedAlgorithm = algorithm;

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
        UpdateClusteringOptions(AlgorithmParameterConfigs.AgglomerativeConfig);

        nudFirstParameter.Value = (decimal)settings.AlgorithmSettings.Threshold;
    }

    private void UpdateClusteringOptionsForKMeans()
    {
        UpdateClusteringOptions(AlgorithmParameterConfigs.KMeansConfig);

        nudFirstParameter.Value = settings.AlgorithmSettings.NumberOfClusters;
        nudSecondParameter.Value = settings.AlgorithmSettings.MaxIterations;
    }

    private void UpdateClusteringOptionsForDBSCAN()
    {
        UpdateClusteringOptions(AlgorithmParameterConfigs.DBSCANConfig);

        nudFirstParameter.Value = (decimal)settings.AlgorithmSettings.Epsilon;
        nudSecondParameter.Value = settings.AlgorithmSettings.MinPoints;
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
        settings.AlgorithmSettings.Threshold = (float)nudFirstParameter.Value;
    }

    private void SaveKMeansSettings()
    {
        settings.AlgorithmSettings.NumberOfClusters = (int)nudFirstParameter.Value;
        settings.AlgorithmSettings.MaxIterations = (int)nudSecondParameter.Value;
    }

    private void SaveDBSCANSettings()
    {
        settings.AlgorithmSettings.Epsilon = (float)nudFirstParameter.Value;
        settings.AlgorithmSettings.MinPoints = (int)nudSecondParameter.Value;
    }
}
