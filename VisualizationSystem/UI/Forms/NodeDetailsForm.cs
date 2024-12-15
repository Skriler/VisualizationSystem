using Microsoft.IdentityModel.Tokens;
using VisualizationSystem.Models.Storages.Results;

namespace VisualizationSystem.UI.Forms;

public partial class NodeDetailsForm : Form
{
    private readonly NodeSimilarityResult similarityResult;
    private readonly Action<string> onNodeCellClick;

    public NodeDetailsForm(NodeSimilarityResult nodeSimilarityResult, Action<string> onNodeCellClick)
    {
        InitializeComponent();

        similarityResult = nodeSimilarityResult;
        this.onNodeCellClick = onNodeCellClick;

        FillWithData();
    }

    public void FillWithData()
    {
        Text = $"Details for {similarityResult.Node.Name}";

        InitializeNodeParametersGrid();
        InitializeSimilarNodesGrid();
    }

    private void InitializeNodeParametersGrid()
    {
        dgvNodeParameters.Rows.Clear();

        foreach (var parameter in similarityResult.Node.Parameters)
        {
            dgvNodeParameters.Rows.Add(parameter.ParameterType.Name, parameter.Value);
        }
    }

    private void InitializeSimilarNodesGrid()
    {
        dgvSimilarNodes.Rows.Clear();

        foreach (var similarNode in similarityResult.SimilarNodes)
        {
            var similarityPercentage = $"{similarNode.SimilarityPercentage:F2}";

            dgvSimilarNodes.Rows.Add(similarNode.Node.Name, similarityPercentage);
        }

        dgvSimilarNodes.Sort(
            dgvSimilarNodes.Columns[1],
            System.ComponentModel.ListSortDirection.Descending
            );

        dgvSimilarNodes.CellDoubleClick += dgvSimilarNodes_CellDoubleClick;
    }

    private void dgvSimilarNodes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex != 0 || e.RowIndex < 0)
            return;

        var nodeName = dgvSimilarNodes.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

        if (nodeName.IsNullOrEmpty())
            return;

        onNodeCellClick?.Invoke(nodeName);
    }
}