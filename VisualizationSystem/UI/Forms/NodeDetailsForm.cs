using Microsoft.IdentityModel.Tokens;
using VisualizationSystem.Models.Domain.Nodes;
using VisualizationSystem.Models.DTOs;
using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.UI.Forms;

public partial class NodeDetailsForm : Form
{
    private readonly Action<string> onNodeCellClick;

    public NodeDetailsForm(NodeSimilarityResult similarityResult, Action<string> onNodeCellClick)
    {
        InitializeComponent();

        this.onNodeCellClick = onNodeCellClick;

        FillWithData(similarityResult);
    }

    public void FillWithData(NodeSimilarityResult similarityResult)
    {
        Text = $"Details for {similarityResult.Node.Name}";

        InitializeNodeParametersGrid(similarityResult.Node.Parameters);
        InitializeSimilarNodesGrid(similarityResult.SimilarNodes);
    }

    private void InitializeNodeParametersGrid(List<NodeParameter> parameters)
    {
        dgvNodeParameters.Rows.Clear();

        var rows = new List<DataGridViewRow>();

        foreach (var parameter in parameters)
        {
            var row = new DataGridViewRow();
            var value = FormatDecimalValue(parameter.Value);
            
            row.CreateCells(
                dgvNodeParameters,
                parameter.ParameterType.Name,
                value
                );
            rows.Add(row);
        }

        dgvNodeParameters.Rows.AddRange(rows.ToArray());
    }

    private void InitializeSimilarNodesGrid(List<SimilarNode> similarNodes)
    {
        dgvSimilarNodes.Rows.Clear();

        var sortedNodes = similarNodes
        .OrderByDescending(sn => sn.SimilarityPercentage)
        .ToList();

        var rows = new List<DataGridViewRow>();

        foreach (var node in sortedNodes)
        {
            var row = new DataGridViewRow();
            var similarityPercentage = Math.Round(node.SimilarityPercentage, 2);
            
            row.CreateCells(
                dgvSimilarNodes, 
                node.Node.Name, 
                similarityPercentage
                );
            rows.Add(row);
        }

        dgvSimilarNodes.Rows.AddRange(rows.ToArray());
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

    private static string FormatDecimalValue(string value)
    {
        if (!decimal.TryParse(value, out decimal number))
            return value;

        if (number == Math.Floor(number))
            return value;

        return Math.Round(number, 2).ToString();
    }
}