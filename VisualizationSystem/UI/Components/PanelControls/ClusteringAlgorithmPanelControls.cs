namespace VisualizationSystem.UI.Components.PanelControls;

public class ClusteringAlgorithmPanelControls : PanelControls
{
    public ComboBox CmbClusterAlgorithm { get; }

    public NumericUpDown NudFirstParameter { get; }

    public NumericUpDown NudSecondParameter { get; }

    public Label LblFirstParameter { get; }

    public Label LblSecondParameter { get; }

    public ClusteringAlgorithmPanelControls(
        Panel panel,
        ComboBox cmbClusterAlgorithm,
        NumericUpDown nudFirstParameter,
        NumericUpDown nudSecondParameter,
        Label lblFirstParameter,
        Label lblSecondParameter
        )
        : base(panel)
    {
        CmbClusterAlgorithm = cmbClusterAlgorithm;
        NudFirstParameter = nudFirstParameter;
        NudSecondParameter = nudSecondParameter;
        LblFirstParameter = lblFirstParameter;
        LblSecondParameter = lblSecondParameter;
    }
}
