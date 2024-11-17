using System.Data;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.SL;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using VisualizationSystem.SL.DAL;
using VisualizationSystem.UI.Components;

namespace VisualizationSystem.UI.Forms;

public partial class MainForm : Form
{
    private static readonly string FileDialogTitle = "Select an Excel File";
    private static readonly string InitialDirectory = "D:\\";
    private static readonly string ExcelFilterPattern
        = "Excel Workbook(*.xlsx)|*.xlsx|Excel 97- Excel 2003 Workbook(*.xls)|*.xls";

    private readonly VisualizationSystemDbContext db;
    private readonly NodeRepository nodeRepository;
    private readonly UserSettingsRepository settingsRepository;

    private NodeTable table;
    private UserSettings settings;

    public MainForm(VisualizationSystemDbContext context)
    {
        InitializeComponent();

        db = context;
        nodeRepository = new NodeRepository(db);
        settingsRepository = new UserSettingsRepository(db);
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        LoadTableNamesToMenu();

        tabControl.Padding = new Point(20, 3);
    }

    private async void uploadExcelFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            if (!TryReadNodeTableFromExcelFile(out table))
                return;

            await nodeRepository.AddTableAsync(table);

            settings = new UserSettings(table);
            await settingsRepository.UpdateAsync(settings);

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
        if (table == null)
        {
            MessageBox.Show("No data to show", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            AddDataGridViewTabPage(table);

            MessageBox.Show("Data showed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while uploading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void buildGraphToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (table == null)
        {
            MessageBox.Show("No data to visualize graph", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var nodeComparer = new NodeComparer(settings);
            var graphBuilder = new GraphBuilder(settings);

            var comparisonResults = nodeComparer.GetSimilarNodes(table);
            var graph = graphBuilder.BuildGraph(comparisonResults, table);

            AddGViewerTabPage(graph, table.Name);

            MessageBox.Show("Graph visualized successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while visualizing graph: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (table == null)
        {
            MessageBox.Show("No data to configure", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using (var settingsForm = new SettingsForm(settings))
        {
            if (settingsForm.ShowDialog() != DialogResult.OK)
                return;

            await settingsRepository.UpdateAsync(settings);
            MessageBox.Show("Settings changed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private async void loadTableToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem selectedItem || string.IsNullOrEmpty(selectedItem.Text))
            return;

        if (selectedItem.Text == table?.Name)
        {
            MessageBox.Show($"Error: Table {table.Name} is already loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        using var loadingForm = new LoadingForm();
        loadingForm.Show();
        loadingForm.BringToFront();
        Enabled = false;

        try
        {
            var tableName = selectedItem.Text;

            table = await nodeRepository.GetByNameAsync(tableName);

            if (await settingsRepository.ExistsAsync(tableName))
            {
                settings = await settingsRepository.GetByTableNameAsync(tableName);
            }
            else
            {
                settings = new UserSettings(table);
                await settingsRepository.AddAsync(settings);
            }

            MessageBox.Show("File uploaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error while uploading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            loadingForm.Close();
            Enabled = true;
            BringToFront();
        }
    }

    private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
    {
        if (tabControl.TabPages[e.Index] is not ClosableTabPage tabPage)
            return;

        tabPage.DrawTab(tabControl.GetTabRect(e.Index), e.Graphics);
    }

    private void tabControl_MouseDown(object sender, MouseEventArgs e)
    {
        for (int i = 0; i < tabControl.TabPages.Count; i++)
        {
            if (tabControl.TabPages[i] is not ClosableTabPage tabPage)
                continue;

            var tabRect = tabControl.GetTabRect(i);

            if (!tabPage.IsCloseIconClicked(tabRect, e.Location))
                continue;

            tabControl.TabPages.RemoveAt(i);
            break;
        }
    }

    private void AddDataGridViewTabPage(NodeTable table)
    {
        var tabPage = new ClosableTabPage("Table: " + table.Name);

        var dataGridView = new NodeTableDataGridView(table)
        {
            Dock = DockStyle.Fill,
        };
        tabPage.Controls.Add(dataGridView);

        tabControl.TabPages.Add(tabPage);
        tabControl.SelectedTab = tabPage;
    }

    private void AddGViewerTabPage(Graph graph, string tabName)
    {
        var tabPage = new ClosableTabPage("Graph: " + tabName);

        GViewer gViewer = new GViewer
        {
            Dock = DockStyle.Fill,
            Graph = graph,
        };
        tabPage.Controls.Add(gViewer);

        tabControl.TabPages.Add(tabPage);
        tabControl.SelectedTab = tabPage;
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
        tableMenuItem.Click += loadTableToolStripMenuItem_Click;
        loadTableToolStripMenuItem.DropDownItems.Add(tableMenuItem);
    }

    private bool TryReadNodeTableFromExcelFile(out NodeTable nodeTable)
    {
        nodeTable = new NodeTable();

        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            InitializeFileDialogParameters(openFileDialog);

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return false;

            string filePath = openFileDialog.FileName;

            nodeTable = ExcelReader.ReadFile(filePath);
        }

        return true;
    }

    private void InitializeFileDialogParameters(OpenFileDialog openFileDialog)
    {
        openFileDialog.Title = FileDialogTitle;
        openFileDialog.InitialDirectory = InitialDirectory;
        openFileDialog.Filter = ExcelFilterPattern;
        openFileDialog.RestoreDirectory = true;
    }
}
