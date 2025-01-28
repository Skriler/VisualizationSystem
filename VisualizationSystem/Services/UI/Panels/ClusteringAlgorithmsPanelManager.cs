using VisualizationSystem.Models.Configs.AlgorithmParameters;
using VisualizationSystem.Models.Domain.Settings;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.Utilities.Clusterers;
using VisualizationSystem.UI.Components.PanelControls;

namespace VisualizationSystem.Services.UI.Panels;

public class ClusteringAlgorithmsPanelManager : PanelManager<ClusteringAlgorithmPanelControls>
{
    private ClusterAlgorithmSettingsData settingsData = default!;
    private int previousAlgorithmIndex;

    private Array algorithms;

    public ClusteringAlgorithmsPanelManager(
        UserSettings settings,
        ClusteringAlgorithmPanelControls controls
        )
        : base(settings, controls)
    {
        algorithms = Enum.GetValues(typeof(ClusterAlgorithm));
    }

    public override void Initialize()
    {
        previousAlgorithmIndex = -1;
        settingsData = new ClusterAlgorithmSettingsData(settings.AlgorithmSettings);

        Controls.CmbClusterAlgorithm.Items.Clear();
        Controls.CmbClusterAlgorithm.Items.AddRange(Enum.GetNames(typeof(ClusterAlgorithm)));

        if (Controls.CmbClusterAlgorithm.Items.Count <= 0)
            return;

        Controls.CmbClusterAlgorithm.SelectedItem = settingsData.SelectedAlgorithm.ToString();

        UpdateContent();
    }


    public override void UpdateContent()
    {
        if(Controls.CmbClusterAlgorithm.SelectedIndex < 0)
            return;

        var algorithm = (ClusterAlgorithm)algorithms.GetValue(Controls.CmbClusterAlgorithm.SelectedIndex);
        settingsData.SelectedAlgorithm = algorithm;

        Action algorithmSetupAction = algorithm switch
        {
            ClusterAlgorithm.HierarchicalAgglomerative => UpdateClusteringOptionsForAgglomerative,
            ClusterAlgorithm.KMeans => UpdateClusteringOptionsForKMeans,
            ClusterAlgorithm.DBSCAN => UpdateClusteringOptionsForDBSCAN,
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), "Unknown clustering algorithm selected")
        };

        algorithmSetupAction();
        previousAlgorithmIndex = Controls.CmbClusterAlgorithm.SelectedIndex;
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

        Controls.NudFirstParameter.Value = (decimal)settingsData.Threshold;
    }

    private void UpdateClusteringOptionsForKMeans()
    {
        UpdateClusteringOptions(AlgorithmParameterConfigs.KMeansConfig);

        Controls.NudFirstParameter.Value = settingsData.NumberOfClusters;
        Controls.NudSecondParameter.Value = settingsData.MaxIterations;
    }

    private void UpdateClusteringOptionsForDBSCAN()
    {
        UpdateClusteringOptions(AlgorithmParameterConfigs.DBSCANConfig);

        Controls.NudFirstParameter.Value = (decimal)settingsData.Epsilon;
        Controls.NudSecondParameter.Value = settingsData.MinPoints;
    }

    private void UpdateClusteringOptions(AlgorithmParameterConfig config)
    {
        Controls.LblFirstParameter.Text = config.FirstParameterLabel;
        Controls.LblSecondParameter.Text = config.SecondParameterLabel;

        Controls.NudFirstParameter.Minimum = config.FirstParameterMin;
        Controls.NudFirstParameter.Maximum = config.FirstParameterMax;

        Controls.NudSecondParameter.Minimum = config.SecondParameterMin;
        Controls.NudSecondParameter.Maximum = config.SecondParameterMax;

        Controls.LblSecondParameter.Visible = config.HasSecondParameter;
        Controls.NudSecondParameter.Visible = config.HasSecondParameter;

        ConfigureNumericUpDown(Controls.NudFirstParameter, config.FirstParameterDecimalPlaces);
        ConfigureNumericUpDown(Controls.NudSecondParameter, config.SecondParameterDecimalPlaces);
    }

    private void ConfigureNumericUpDown(NumericUpDown nud, int decimalPlaces)
    {
        nud.DecimalPlaces = decimalPlaces;
        nud.Increment = (decimal)Math.Pow(10, -decimalPlaces);
    }

    private void SaveAgglomerativeSettings()
    {
        settingsData.Threshold = (float)Controls.NudFirstParameter.Value;
    }

    private void SaveKMeansSettings()
    {
        settingsData.NumberOfClusters = (int)Controls.NudFirstParameter.Value;
        settingsData.MaxIterations = (int)Controls.NudSecondParameter.Value;
    }

    private void SaveDBSCANSettings()
    {
        settingsData.Epsilon = (float)Controls.NudFirstParameter.Value;
        settingsData.MinPoints = (int)Controls.NudSecondParameter.Value;
    }
}
