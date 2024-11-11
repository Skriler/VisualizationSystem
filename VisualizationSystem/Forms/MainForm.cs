using System.Data;
using Microsoft.IdentityModel.Tokens;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.ViewModels;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.SL;
using VisualizationSystem.SL.DAL;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Windows.Documents;

namespace VisualizationSystem.Forms;

public partial class MainForm : Form
{
    private static readonly string FileDialogTitle = "Select an Excel File";
    private static readonly string InitialDirectory = "D:\\";
    private static readonly string ExcelFilterPattern
        = "Excel Workbook(*.xlsx)|*.xlsx|Excel 97- Excel 2003 Workbook(*.xls)|*.xls";

    private readonly VisualizationSystemDbContext db;
    private readonly NodeRepository nodeRepository;
    private readonly ComparisonSettings comparisonSettings;
    private readonly NodeComparer nodeComparer;
    private readonly GraphBuilder graphBuilder;

    private NodeTable nodeTable = new NodeTable();

    public MainForm(VisualizationSystemDbContext context)
    {
        InitializeComponent();

        db = context;
        nodeRepository = new NodeRepository(db);
        comparisonSettings = new ComparisonSettings();
        nodeComparer = new NodeComparer(comparisonSettings);
        graphBuilder = new GraphBuilder(comparisonSettings);
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        LoadTableNamesToMenu();
    }

    private async void uploadExcelFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            LoadExcelData();
            await nodeRepository.AddTableAsync(nodeTable);
            //AddTableToolStripMenuItem(nodeTable.Name);

            MessageBox.Show("File uploaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while uploading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void showToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (nodeTable.NodeObjects.IsNullOrEmpty())
        {
            MessageBox.Show("No data to show", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            ShowTableData(nodeTable);

            MessageBox.Show("Data showed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while uploading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void tableToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem selectedItem || selectedItem.Text == null)
            return;

        try
        {
            nodeTable = await nodeRepository.GetByNameAsync(selectedItem.Text);

            MessageBox.Show("File uploaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while uploading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void buildGraphToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (nodeTable.NodeObjects.IsNullOrEmpty())
        {
            MessageBox.Show("No data to visualize graph", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var comparisonResults = nodeComparer.GetSimilarNodes(nodeTable);
            gViewer.Graph = graphBuilder.BuildGraph(comparisonResults, nodeTable);

            MessageBox.Show("Graph visualized successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while visualizing graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (nodeTable.NodeObjects.IsNullOrEmpty())
        {
            MessageBox.Show("No data to configure", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using (var settingsForm = new SettingsForm(comparisonSettings))
        {
            if (settingsForm.ShowDialog() != DialogResult.OK)
                return;

            nodeComparer.UpdateSettings(comparisonSettings);
            graphBuilder.Equals(comparisonSettings);
            MessageBox.Show("Settings changed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private async void LoadTableNamesToMenu()
    {
        loadTableToolStripMenuItem.DropDownItems.Clear();

        var tables = await nodeRepository.GetAllAsync();

        if (!tables.Any())
        {
            loadTableToolStripMenuItem.Enabled = false;
            return;
        }
            
        loadTableToolStripMenuItem.Enabled = true;

        var tableNames = tables
            .Select(x => x.Name)
            .ToList();

        foreach (var table in tables)
        {
            AddTableToolStripMenuItem(table.Name);
        }
    }

    private void AddTableToolStripMenuItem(string tableName)
    {
        ToolStripMenuItem tableMenuItem = new ToolStripMenuItem(tableName);
        tableMenuItem.Click += tableToolStripMenuItem_Click;
        loadTableToolStripMenuItem.DropDownItems.Add(tableMenuItem);
    }

    private void LoadExcelData()
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            InitializeFileDialogParameters(openFileDialog);

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string filePath = openFileDialog.FileName;

            nodeTable = ExcelReader.ReadFile(filePath);
        }
    }

    private void ShowTableData(NodeTable table)
    {
        var headers =
            table
            .ParameterTypes
            .Select(p => p.Name)
            .Where(name => name != null)
            .Cast<string>();

        NodeObjectViewModel.InitializeHeaders(headers);

        var nodeViewModels = nodeTable.NodeObjects
            .Select(n => new NodeObjectViewModel(n))
            .ToList();

        var dataTable = CreateDataTable(nodeViewModels);

        dataGridViewNodes.Visible = true;
        dataGridViewNodes.DataSource = dataTable;

        DisableDataGridViewInteractions();
    }

    private DataTable CreateDataTable(List<NodeObjectViewModel> nodeViewModels)
    {
        var dataTable = new DataTable();

        foreach (var header in NodeObjectViewModel.Headers)
        {
            dataTable.Columns.Add(header);
        }

        DataRow row;
        foreach (var node in nodeViewModels)
        {
            row = dataTable.NewRow();

            for (int i = 0; i < node.Parameters.Count; ++i)
            {
                row[i] = node.Parameters[i];
            }

            dataTable.Rows.Add(row);
        }

        return dataTable;
    }

    private void InitializeFileDialogParameters(OpenFileDialog openFileDialog)
    {
        openFileDialog.Title = FileDialogTitle;
        openFileDialog.InitialDirectory = InitialDirectory;
        openFileDialog.Filter = ExcelFilterPattern;
        openFileDialog.RestoreDirectory = true;
    }

    private void DisableDataGridViewInteractions()
    {
        dataGridViewNodes.ReadOnly = true;
        dataGridViewNodes.AllowUserToAddRows = false;
        dataGridViewNodes.AllowUserToDeleteRows = false;
        dataGridViewNodes.AllowUserToResizeRows = false;
        dataGridViewNodes.AllowUserToResizeColumns = false;
        dataGridViewNodes.AllowUserToOrderColumns = false;
        //dataGridViewNodes.RowHeadersVisible = false;
    }
}
