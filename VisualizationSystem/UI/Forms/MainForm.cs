using VisualizationSystem.Models.Entities;
using VisualizationSystem.Services.Utilities;
using VisualizationSystem.Services.DAL;
using VisualizationSystem.Services.UI;
using VisualizationSystem.UI.Components;

namespace VisualizationSystem.UI.Forms;

public partial class MainForm : Form
{
    private readonly NodeTableRepository nodeRepository;
    private readonly UserSettingsRepository settingsRepository;

    private readonly FileService fileService;
    private readonly TabControlService tabControlService;

    private NodeTable? nodeTable;
    private UserSettings userSettings;

    public MainForm(VisualizationSystemDbContext context)
    {
        InitializeComponent();

        nodeRepository = new NodeTableRepository(context);
        settingsRepository = new UserSettingsRepository(context);

        fileService = new FileService();
        tabControlService = new TabControlService(tabControl);
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        await LoadTableNamesToMenuAsync();

        tabControl.Padding = new Point(20, 3);
    }

    private async void uploadExcelFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            if (!fileService.TryReadNodeTableFromExcelFile(out nodeTable))
                return;

            await nodeRepository.AddTableAsync(nodeTable);

            userSettings = new UserSettings(nodeTable);
            await settingsRepository.UpdateAsync(userSettings);

            //AddTableToolStripMenuItem(nodeTable.Name);

            ShowSuccess("File uploaded successfully!");
        }
        catch (Exception ex)
        {
            ShowError("Error while uploading data", ex);
        }
    }

    private void showToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (nodeTable == null)
        {
            ShowWarning("No data to show");
            return;
        }

        try
        {
            tabControlService.AddDataGridViewTabPage(nodeTable);
        }
        catch (Exception ex)
        {
            ShowError("Error while showing data", ex);
        }
    }

    private void buildGraphToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (nodeTable == null)
        {
            ShowWarning("No data to visualize graph");
            return;
        }

        try
        {
            var nodeComparer = new NodeComparer(userSettings);
            var graphBuilder = new GraphBuilder(userSettings);

            var comparisonResults = nodeComparer.GetSimilarNodes(nodeTable);
            var graph = graphBuilder.BuildGraph(comparisonResults, nodeTable);

            tabControlService.AddGViewerTabPage(graph, nodeTable.Name);

        }
        catch (Exception ex)
        {
            ShowError("Error while visualizing graph", ex);
        }
    }

    private async void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (nodeTable == null)
        {
            ShowWarning("No data to configure");
            return;
        }

        using (var settingsForm = new SettingsForm(userSettings))
        {
            if (settingsForm.ShowDialog() != DialogResult.OK)
                return;

            await settingsRepository.UpdateAsync(userSettings);
        }
    }

    private async void loadTableToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem selectedItem || string.IsNullOrEmpty(selectedItem.Text))
            return;

        if (selectedItem.Text == nodeTable?.Name)
        {
            ShowWarning($"Table {nodeTable.Name} is already loaded");
            return;
        }

        using var loadingForm = new LoadingForm();
        loadingForm.Show();
        loadingForm.BringToFront();
        Enabled = false;

        try
        {
            var tableName = selectedItem.Text;

            nodeTable = await nodeRepository.GetByNameAsync(tableName);

            if (await settingsRepository.ExistsAsync(tableName))
            {
                userSettings = await settingsRepository.GetByTableNameAsync(tableName);
            }
            else
            {
                userSettings = new UserSettings(nodeTable);
                await settingsRepository.AddAsync(userSettings);
            }

            ShowSuccess("File uploaded successfully!");
        }
        catch (Exception ex)
        {
            ShowError("Error while uploading data", ex);
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
        for (var i = 0; i < tabControl.TabPages.Count; i++)
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

    private async Task LoadTableNamesToMenuAsync()
    {
        loadTableToolStripMenuItem.DropDownItems.Clear();

        var tables = await nodeRepository.GetAllAsync();

        if (tables.Count <= 0)
        {
            loadTableToolStripMenuItem.Enabled = false;
            return;
        }

        loadTableToolStripMenuItem.Enabled = true;

        var tableNames = tables
            .Select(x => x.Name)
            .ToList();

        foreach (var tableName in tableNames)
        {
            AddTableToolStripMenuItem(tableName);
        }
    }

    private void AddTableToolStripMenuItem(string tableName)
    {
        var tableMenuItem = new ToolStripMenuItem(tableName);
        tableMenuItem.Click += loadTableToolStripMenuItem_Click;

        loadTableToolStripMenuItem.DropDownItems.Add(tableMenuItem);
    }

    private static void ShowSuccess(string message)
    {
        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static void ShowWarning(string message)
    {
        MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private static void ShowError(string message, Exception ex)
    {
        MessageBox.Show($"{message}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
