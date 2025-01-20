using VisualizationSystem.UI.Components.TabPages;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.FileSystem.ExcelHandlers;
using VisualizationSystem.Services.Utilities.Settings;
using VisualizationSystem.Services.Utilities.Graphs.Managers;
using VisualizationSystem.Models.Domain.Graphs;
using VisualizationSystem.Services.Utilities.Plots;
using VisualizationSystem.Services.UI.TabPages;
using VisualizationSystem.Services.DAL.Repositories;

namespace VisualizationSystem.UI.Forms;

public partial class MainForm : Form
{
    private readonly string FormTitle;

    private readonly NodeTableRepository nodeTableRepository;

    private readonly TabControlService tabControlService;
    private readonly ExcelDataImporter fileService;
    private readonly ISettingsManager userSettingsManager;
    private readonly GraphCreationManager<ExtendedGraph> graphCreationManager;
    private readonly GraphSaveManager graphSaveManager;
    private readonly PlotCreationManager plotCreationManager;

    private NodeTable nodeTable = default!;

    public MainForm(
        NodeTableRepository nodeTableRepository,
        TabControlService tabControlService,
        ExcelDataImporter fileService,
        ISettingsManager userSettingsManager,
        GraphCreationManager<ExtendedGraph> graphCreationManager,
        GraphSaveManager graphSaveManager,
        PlotCreationManager plotCreationManager
        )
    {
        InitializeComponent();

        FormTitle = Text;

        this.nodeTableRepository = nodeTableRepository;

        this.tabControlService = tabControlService;
        this.fileService = fileService;
        this.userSettingsManager = userSettingsManager;
        this.graphCreationManager = graphCreationManager;
        this.graphSaveManager = graphSaveManager;
        this.plotCreationManager = plotCreationManager;

        tabControlService.Initialize(tabControl);
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
            tabControlService.AddTab(nodeTable);
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
            ShowWarning("No data to build graph");
            return;
        }

        try
        {
            var graph = await graphCreationManager.BuildGraphAsync(nodeTable);

            tabControlService.AddTab(graph);
        }
        catch (Exception ex)
        {
            ShowError("Error while building graph", ex);
        }
    }

    private async void buildPlotToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (nodeTable == null)
        {
            ShowWarning("No data to build plot");
            return;
        }

        try
        {
            var plot = await plotCreationManager.BuildClusteredPlotAsync(nodeTable);

            tabControlService.AddTab(plot);
        }
        catch (Exception ex)
        {
            ShowError("Error while building plot", ex);
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
            await graphSaveManager.SaveGraphAsync(nodeTable);
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

        if (await userSettingsManager.TryOpenSettingsForm())
        {
            //await UpdateGraphIfNeededAsync();
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

        //await UpdateGraphIfNeededAsync();
    }

    private async Task OnDeleteTable(string tableName)
    {
        await nodeTableRepository.DeleteAsync(tableName);

        datasetsToolStripMenuItem.DropDownItems.RemoveByKey(tableName);
        tabControlService.RemoveRelatedTabPages(tableName);

        await LoadTableNamesToMenuAsync();
    }

    //private async Task UpdateGraphIfNeededAsync()
    //{
    //    var graph = await graphCreationManager.BuildGraphAsync(nodeTable);

    //    tabControlService.UpdateDataGridViewTabPageIfOpen(nodeTable);
    //    tabControlService.UpdateGViewerTabPageIfOpen(graph, nodeTable.Name);
    //}

    private void UpdateFormTitle()
    {
        Text = nodeTable == null ? FormTitle : FormTitle + $" (Current dataset: {nodeTable.Name})";
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
