using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Windows.Forms;
using VisualizationSystem.Models.Storages;

namespace VisualizationSystem.UI.Forms;

public partial class ColumnSelectionForm : Form
{
    private readonly List<string> columnNames;

    public ColumnSelectionResult SelectedColumn { get; private set; }

    public ColumnSelectionForm(string columnHeader, List<string> columnNames)
    {
        InitializeComponent();

        this.columnNames = columnNames;

        lblSelectColumn.Text = $"Select {columnHeader} column";
        InitializeComboBox();
    }

    private void InitializeComboBox()
    {
        cmbColumnNames.Items.Clear();

        foreach (var columnName in columnNames)
        {
            cmbColumnNames.Items.Add(columnName);
        }

        cmbColumnNames.SelectedIndex = 0;
    }

    private void btnSubmit_Click(object sender, EventArgs e)
    {
        if (cmbColumnNames.SelectedIndex <= -1)
            throw new InvalidOperationException("A column must be selected");

        SelectedColumn = new ColumnSelectionResult(
            cmbColumnNames.SelectedIndex, 
            cmbColumnNames.SelectedItem.ToString()
            );

        DialogResult = DialogResult.OK;
        Close();
    }
}
