using VisualizationSystem.Services.UI;
using VisualizationSystem.UI.Components.TabPages;
using VisualizationSystem.Services.Utilities.ExcelHandlers;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.DAL;
using VisualizationSystem.Services.Utilities.Managers;
using VisualizationSystem.Models.Storages.Results;

namespace VisualizationSystem.UI.Forms;

public partial class MainForm : Form
{
    private readonly string FormTitle;

    private readonly NodeTableRepository nodeTableRepository;

    private readonly ExcelDataImporter fileService;
    private readonly UserSettingsManager userSettingsManager;
    private readonly GraphManager graphManager;

    private readonly TabControlManager tabControlService;

    private NodeTable nodeTable;

    public MainForm(
        NodeTableRepository nodeTableRepository,
        ExcelDataImporter fileService,
        UserSettingsManager userSettingsManager,
        GraphManager graphManager
        )
    {
        InitializeComponent();

        FormTitle = Text;

        this.nodeTableRepository = nodeTableRepository;

        this.fileService = fileService;
        this.userSettingsManager = userSettingsManager;
        this.graphManager = graphManager;

        tabControlService = new TabControlManager(tabControl);
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        await LoadTableNamesToMenuAsync();

        tabControl.Padding = new Point(20, 3);
        saveGraphImageToolStripMenuItem.Visible = false;
    }

    private async void uploadToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (!await TryLoadTableFromFileAsync())
            return;

        await LoadTableNamesToMenuAsync();
        await ApplySettingsToComponentsAsync();
        UpdateFormTitle();

        ShowSuccess("File uploaded successfully!");
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

    private async void buildGraphToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (nodeTable == null)
        {
            ShowWarning("No data to visualize graph");
            return;
        }

        try
        {
            var graph = userSettingsManager.UseClustering() ?
                await graphManager.BuildClusteredGraphAsync(nodeTable) :
                graphManager.BuildGraph(nodeTable);

            tabControlService.AddGViewerTabPage(graph, nodeTable.Name, OpenNodeDetailsForm);
        }
        catch (Exception ex)
        {
            ShowError("Error while visualizing graph", ex);
        }
    }

    private async void saveGraphImageToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (nodeTable == null)
        {
            ShowWarning("No data to save");
            return;
        }

        try
        {
            if (userSettingsManager.UseClustering())
            {
                await graphManager.SaveClusteredGraphAsync(nodeTable);
            }
            else
            {
                await graphManager.SaveGraphAsync(nodeTable);
            }
        }
        catch (Exception ex)
        {
            ShowError("Error while showing data", ex);
        }
    }

    private async void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (nodeTable == null)
        {
            ShowWarning("No data to configure");
            return;
        }

        var userSettings = userSettingsManager.UserSettings;

        using (var settingsForm = new SettingsForm(userSettings))
        {
            if (settingsForm.ShowDialog() != DialogResult.OK)
                return;

            await userSettingsManager.UpdateAsync(userSettings);
            await ApplySettingsToComponentsAsync();
        }
    }

    private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
    {
        if (tabControl.TabPages[e.Index] is not ClosableTabPageBase tabPage)
            return;

        tabPage.DrawTab(tabControl.GetTabRect(e.Index), e.Graphics);
    }

    private void tabControl_MouseDown(object sender, MouseEventArgs e)
    {
        for (var i = 0; i < tabControl.TabPages.Count; i++)
        {
            if (tabControl.TabPages[i] is not ClosableTabPageBase tabPage)
                continue;

            var tabRect = tabControl.GetTabRect(i);

            if (!tabPage.IsCloseIconClicked(tabRect, e.Location))
                continue;

            tabControlService.RemoveTabPage(tabPage);
            break;
        }
    }

    private async Task<bool> TryLoadTableFromFileAsync()
    {
        try
        {
            if (!fileService.TryReadNodeTableFromExcelFile(out var nodeTables))
                return false;

            if (!nodeTables.Any())
                return false;

            await nodeTableRepository.AddListAsync(nodeTables);
            nodeTable = nodeTables.Last();

            await userSettingsManager.LoadAsync(nodeTable);
        }
        catch (Exception ex)
        {
            ShowError("Error while uploading data", ex);
            return false;
        }

        return true;
    }

    private async Task LoadTableNamesToMenuAsync()
    {
        datasetsToolStripMenuItem.DropDownItems.Clear();

        var nodeTables = await nodeTableRepository.GetAllAsync();
        var tableNames = nodeTables.Select(x => x.Name).ToList();

        if (!tableNames.Any())
        {
            datasetsToolStripMenuItem.Enabled = false;
            return;
        }

        tableNames.ForEach(AddTableToolStripMenuItem);
        datasetsToolStripMenuItem.Enabled = true;
    }

    private void AddTableToolStripMenuItem(string tableName)
    {
        var tableMenuItem = new ToolStripMenuItem(tableName);

        tableMenuItem.DropDownItems.Add(
            CreateMenuItem("Select", async () => await HandleLoadTableAsync(tableName))
            );
        tableMenuItem.DropDownItems.Add(
            CreateMenuItem("Remove", async () => await HandleDeleteTableAsync(tableName))
            );

        datasetsToolStripMenuItem.DropDownItems.Add(tableMenuItem);
    }

    private ToolStripMenuItem CreateMenuItem(string text, Func<Task> onClickAction)
    {
        var menuItem = new ToolStripMenuItem(text);
        menuItem.Click += async (sender, e) => await onClickAction();

        return menuItem;
    }

    private async Task HandleLoadTableAsync(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
            return;

        if (nodeTable?.Name == tableName)
        {
            ShowWarning($"Table {tableName} is already loaded");
            return;
        }

        await ExecuteTableOperationAsync(tableName, "Loading table", OnLoadTable);
    }

    private async Task HandleDeleteTableAsync(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
            return;

        if (nodeTable?.Name == tableName)
        {
            ShowWarning($"Cannot delete table {tableName} while loaded");
            return;
        }

        await ExecuteTableOperationAsync(tableName, "Deleting table", OnDeleteTable);
    }

    private async Task ExecuteTableOperationAsync(string tableName, string operationDescription, Func<string, Task> operation)
    {
        using var loadingForm = new LoadingForm(operationDescription);
        loadingForm.Show();
        loadingForm.BringToFront();

        Enabled = false;

        try
        {
            await operation(tableName);
            ShowSuccess("The operation was successful!");
        }
        catch (Exception ex)
        {
            ShowError($"{operationDescription} failed", ex);
        }
        finally
        {
            loadingForm.Close();

            Enabled = true;
            BringToFront();
            UpdateFormTitle();
        }
    }

    private async Task OnLoadTable(string tableName)
    {
        nodeTable = await nodeTableRepository.GetByNameAsync(tableName);
        await userSettingsManager.LoadAsync(nodeTable);

        await ApplySettingsToComponentsAsync();
    }

    private async Task OnDeleteTable(string tableName)
    {
        await nodeTableRepository.DeleteTableAsync(tableName);

        datasetsToolStripMenuItem.DropDownItems.RemoveByKey(tableName);
        tabControlService.RemoveRelatedTabPages(tableName);

        await LoadTableNamesToMenuAsync();
    }

    private async Task ApplySettingsToComponentsAsync()
    {
        graphManager.UpdateSettings(userSettingsManager.UserSettings);

        if (graphManager.Graph != null)
        {
            var graph = userSettingsManager.UseClustering() ?
                await graphManager.BuildClusteredGraphAsync(nodeTable) :
                graphManager.BuildGraph(nodeTable);
        }

        tabControlService.UpdateDataGridViewTabPageIfOpen(nodeTable);
        tabControlService.UpdateGViewerTabPageIfOpen(graphManager.Graph, nodeTable.Name);
    }

    private void OpenNodeDetailsForm(NodeSimilarityResult nodeSimilarityResult)
    {
        if (nodeSimilarityResult == null)
            return;

        var detailsForm = new NodeDetailsForm(nodeSimilarityResult, OpenNodeDetailsForm);
        detailsForm.Show();
        detailsForm.BringToFront();
    }

    private void UpdateFormTitle()
    {
        Text = nodeTable == null ? FormTitle : FormTitle + $" (Current table: {nodeTable.Name})";
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
