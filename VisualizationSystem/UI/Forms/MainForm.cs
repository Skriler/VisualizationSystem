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
    private readonly PlotCreationManager plotCreationManager;

    private NodeTable nodeTable = default!;

    public MainForm(
        NodeTableRepository nodeTableRepository,
        TabControlService tabControlService,
        ExcelDataImporter fileService,
        ISettingsManager userSettingsManager,
        GraphCreationManager<ExtendedGraph> graphCreationManager,
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
        this.plotCreationManager = plotCreationManager;

        tabControlService.Initialize(tabControl);
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        await LoadTableNamesToMenuAsync();

        tabControl.Padding = new Point(20, 3);
    }

    private async void uploadToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (!await TryLoadTableFromFileAsync())
            return;

        await LoadTableNamesToMenuAsync();
        UpdateFormTitle();
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
            await ExecuteTableOperationAsync(nodeTable.Name, "Building graph", OnBuildGraph);
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
            await ExecuteTableOperationAsync(nodeTable.Name, "Building plot", OnBuildPlot);
        }
        catch (Exception ex)
        {
            ShowError("Error while building plot", ex);
        }
    }

    private async void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (nodeTable == null)
        {
            ShowWarning("No data to configure");
            return;
        }

        await userSettingsManager.TryOpenSettingsForm();
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

            if (nodeTables.Count == 0)
                return false;

            var newTables = await ProcessDuplicateTables(nodeTables);

            if (newTables.Count == 0)
                return false;

            await UploadNewTablesAsync(newTables);

        }
        catch (Exception ex)
        {
            ShowError("Error while uploading data", ex);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Uploads new tables to the repository, sets last one as active and creates user settings for it.
    /// </summary>
    /// <param name="newTables">List of new tables to upload.</param>
    /// <returns>No value</returns>
    private async Task UploadNewTablesAsync(List<NodeTable> newTables)
    {
        await nodeTableRepository.AddListAsync(newTables);

        nodeTable = newTables.Last();
        await userSettingsManager.LoadAsync(nodeTable);

        var newNames = string.Join(", ", newTables.Select(t => t.Name));
        ShowSuccess($"Tables: {newNames} uploaded successfully!");
    }

    /// <summary>
    /// Filters out duplicates from the provided node tables, notify about them and returns only new ones.
    /// </summary>
    /// <param name="nodeTables">List of node tables to process.</param>
    /// <returns>List of new node tables.</returns>
    private async Task<List<NodeTable>> ProcessDuplicateTables(List<NodeTable> tables)
    {
        var existingTables = await nodeTableRepository.GetAllNamesAsync();

        var groupedTables = tables.ToLookup(t => existingTables.Contains(t.Name));

        var newTables = groupedTables[false];
        var duplicateTables = groupedTables[true];

        if (duplicateTables.Any())
        {
            var duplicateNames = string.Join(", ", duplicateTables.Select(t => t.Name));
            ShowWarning($"Tables already exist: {duplicateNames}");
        }

        return newTables.ToList();
    }

    private async Task LoadTableNamesToMenuAsync()
    {
        datasetsToolStripMenuItem.DropDownItems.Clear();

        var tableNames = await nodeTableRepository.GetAllNamesAsync();

        if (tableNames.Count == 0)
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
    }

    private async Task OnDeleteTable(string tableName)
    {
        await nodeTableRepository.DeleteAsync(tableName);

        datasetsToolStripMenuItem.DropDownItems.RemoveByKey(tableName);
        tabControlService.RemoveRelatedTabPages(tableName);

        await LoadTableNamesToMenuAsync();
    }

    private async Task OnBuildGraph(string tableName)
    {
        if (tableName != nodeTable.Name)
            throw new ArgumentException("Incorrect table name");

        var graph = await Task.Run(() => graphCreationManager.BuildGraphAsync(nodeTable));

        tabControlService.AddTab(graph);
    }

    private async Task OnBuildPlot(string tableName)
    {
        if (tableName != nodeTable.Name)
            throw new ArgumentException("Incorrect table name");

        var plot = await Task.Run(() => plotCreationManager.BuildClusteredPlotAsync(nodeTable));

        tabControlService.AddTab(plot);
    }

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
