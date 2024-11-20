using VisualizationSystem.Models.Storages;

namespace VisualizationSystem.UI.Forms;

public partial class NodeDetailsForm : Form
{
    private readonly NodeSimilarityResult similarityResult;

    public NodeDetailsForm(NodeSimilarityResult nodeSimilarityResult)
    {
        InitializeComponent();

        similarityResult = nodeSimilarityResult;
        Text = $"Details for {similarityResult.Node.Name}";

        InitializeControls();
    }

    public void InitializeControls()
    {
        lblNodeName.Text = $"Node Name: {similarityResult.Node.Name}";

        lstNodeParameters.Items.Clear();
        foreach (var parameter in similarityResult.Node.Parameters)
        {
            lstNodeParameters.Items.Add($"{parameter.ParameterType.Name}: {parameter.Value}");
        }

        lstSimilarNodes.Items.Clear();
        foreach (var similarNode in similarityResult.SimilarNodes)
        {
            lstSimilarNodes.Items.Add($"{similarNode.Node.Name} - Similarity: {similarNode.SimilarityPercentage}%");
        }
    }
}