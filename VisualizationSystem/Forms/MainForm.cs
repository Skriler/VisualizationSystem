using System.Data;
using Microsoft.IdentityModel.Tokens;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.ViewModels;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.SL;
using VisualizationSystem.SL.DAL;

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

    private async void MainForm_Load(object sender, EventArgs e)
    {
        LoadTableNamesToMenu();
    }

    private async void uploadExcelFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        LoadExcelData();
        await SaveDataToDatabaseAsync();
    }

    private void showToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (nodeTable.NodeObjects.IsNullOrEmpty())
        {
            MessageBox.Show("No data to show", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var headers =
            nodeTable
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

    private async void TableMenuItem_Click(object sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem selectedItem)
            return;

        await LoadNodeTable(selectedItem.Text);
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
            MessageBox.Show("Nodes compared successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while visualizing graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        ToolStripMenuItem tableMenuItem;
        foreach (var table in tables)
        {
            tableMenuItem = new ToolStripMenuItem(table.Name);
            tableMenuItem.Click += TableMenuItem_Click;
            loadTableToolStripMenuItem.DropDownItems.Add(tableMenuItem);
        }
    }

    private async Task LoadNodeTable(string tableName)
    {
        nodeTable =  await nodeRepository.GetByNameAsync(tableName);
    }

    private void LoadExcelData()
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            InitializeFileDialogParameters(openFileDialog);

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            string filePath = openFileDialog.FileName;

            try
            {
                nodeTable = ExcelReader.ReadFile(filePath);
                MessageBox.Show("Data read successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
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

    private async Task SaveDataToDatabaseAsync()
    {
        try
        {
            await nodeRepository.AddTableAsync(nodeTable);
            MessageBox.Show("Data saved to database successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while saving data to database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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
