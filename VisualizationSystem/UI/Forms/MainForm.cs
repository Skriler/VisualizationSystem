﻿using Microsoft.Msagl.Drawing;
using VisualizationSystem.Services.DAL;
using VisualizationSystem.Services.UI;
using VisualizationSystem.Services.Utilities.Clusterers;
using VisualizationSystem.UI.Components.TabPages;
using VisualizationSystem.Services.Utilities.ExcelHandlers;
using VisualizationSystem.Services.Utilities.GraphBuilders;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.Utilities.Comparers;
using VisualizationSystem.Services.Utilities.Factories;

namespace VisualizationSystem.UI.Forms;

public partial class MainForm : Form
{
    private readonly string FormTitle;

    private readonly NodeTableRepository nodeRepository;
    private readonly UserSettingsRepository settingsRepository;

    private readonly ExcelDataImporter fileService;
    private readonly NodeComparisonManager nodeComparisonManager;
    private readonly GraphSaveManager graphSaveManager;
    private readonly IGraphBuilder<Graph> graphBuilder;

    private readonly UserSettingsFactory userSettingsFactory;

    private readonly TabControlManager tabControlService;

    private NodeTable? nodeTable;
    private UserSettings? userSettings;
    private Graph? graph;

    public MainForm(
        NodeTableRepository nodeRepository,
        UserSettingsRepository settingsRepository,
        ExcelDataImporter fileService,
        NodeComparisonManager nodeComparisonManager,
        GraphSaveManager graphSaveManager,
        IGraphBuilder<Graph> graphBuilder,
        UserSettingsFactory userSettingsFactory
        )
    {
        InitializeComponent();

        FormTitle = Text;

        this.nodeRepository = nodeRepository;
        this.settingsRepository = settingsRepository;
        this.fileService = fileService;
        this.nodeComparisonManager = nodeComparisonManager;
        this.graphSaveManager = graphSaveManager;
        this.graphBuilder = graphBuilder;
        this.userSettingsFactory = userSettingsFactory;

        tabControlService = new TabControlManager(tabControl);
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        await LoadTableNamesToMenuAsync();

        tabControl.Padding = new Point(20, 3);

        saveGraphImageToolStripMenuItem.Visible = false;
    }

    private async void uploadExcelFileToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (!await TryUploadFileAsync())
            return;

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
            AddDataGridViewTab();
        }
        catch (Exception ex)
        {
            ShowError("Error while showing data", ex);
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
            if (userSettings.UseClustering)
            {
                await graphSaveManager.SaveGraphAsync(nodeTable.Name, nodeTable.NodeObjects, nodeComparisonManager.Clusters);
            }
            else
            {
                await graphSaveManager.SaveGraphAsync(nodeTable.Name, nodeComparisonManager.SimilarityResults);
            }
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
            AddGraphTab();
        }
        catch (Exception ex)
        {
            ShowError("Error while visualizing graph", ex);
        }
    }

    private async void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (userSettings == null)
        {
            ShowWarning("No data to configure");
            return;
        }

        using (var settingsForm = new SettingsForm(userSettings))
        {
            if (settingsForm.ShowDialog() != DialogResult.OK)
                return;

            await settingsRepository.UpdateAsync(userSettings);
            ApplySettingsToComponents();
        }
    }

    private async void LoadTable(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
            return;

        if (tableName == nodeTable?.Name)
        {
            ShowWarning($"Table {nodeTable.Name} is already loaded");
            return;
        }

        await LoadTableAsync(tableName);
    }

    private async void DeleteTable(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
            return;

        if (tableName == nodeTable?.Name)
        {
            ShowWarning($"Table {nodeTable.Name} is already loaded");
            return;
        }

        await DeleteTableAsync(tableName);
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

    private async Task<bool> TryUploadFileAsync()
    {
        try
        {
            if (!fileService.TryReadNodeTableFromExcelFile(out var tables))
                return false;

            await AddLoadedNodeTablesAsync(tables);

            nodeTable = tables.Last();
            await LoadUserSettingsAsync();
            UpdateFormTitle();
        }
        catch (Exception ex)
        {
            ShowError("Error while uploading data", ex);
            return false;
        }

        return true;
    }

    private async Task AddLoadedNodeTablesAsync(List<NodeTable> loadedTables)
    {
        if (loadedTables.Count < 0)
            return;

        foreach (var loadedTable in loadedTables)
        {
            await nodeRepository.AddTableAsync(loadedTable);
        }

        await LoadTableNamesToMenuAsync();
        tablesToolStripMenuItem.Enabled = true;
    }

    private async Task LoadTableNamesToMenuAsync()
    {
        tablesToolStripMenuItem.DropDownItems.Clear();

        var tables = await nodeRepository.GetAllAsync();

        if (tables.Count <= 0)
        {
            tablesToolStripMenuItem.Enabled = false;
            return;
        }

        var tableNames = tables
            .Select(x => x.Name)
            .ToList();

        foreach (var tableName in tableNames)
        {
            AddTableToolStripMenuItem(tableName);
        }

        tablesToolStripMenuItem.Enabled = true;
    }

    private void AddTableToolStripMenuItem(string tableName)
    {
        var tableMenuItem = new ToolStripMenuItem(tableName);

        tableMenuItem.DropDownItems.Add(
            CreateMenuItem("Load", () => LoadTable(tableName))
            );
        tableMenuItem.DropDownItems.Add(
            CreateMenuItem("Delete", () => DeleteTable(tableName))
            );

        tablesToolStripMenuItem.DropDownItems.Add(tableMenuItem);
    }

    private ToolStripMenuItem CreateMenuItem(string text, Action onClickAction)
    {
        var menuItem = new ToolStripMenuItem(text);
        menuItem.Click += (sender, e) => onClickAction();

        return menuItem;
    }

    private void AddDataGridViewTab()
    {
        tabControlService.AddDataGridViewTabPage(nodeTable);
    }

    private void AddGraphTab()
    {
        CreateGraph();
        tabControlService.AddGViewerTabPage(graph, nodeTable.Name, OpenNodeDetailsForm);
    }

    private void CreateGraph()
    {
        if (userSettings.UseClustering)
        {
            nodeComparisonManager.CalculateClusters(nodeTable.NodeObjects);
            graph = graphBuilder.Build(nodeTable.Name, nodeTable.NodeObjects, nodeComparisonManager.Clusters);
        }
        else
        {
            nodeComparisonManager.CalculateSimilarNodes(nodeTable.NodeObjects);
            graph = graphBuilder.Build(nodeTable.Name, nodeComparisonManager.SimilarityResults);
        }
    }

    private async Task LoadTableAsync(string tableName)
    {
        using var loadingForm = new LoadingForm("Loading table...");
        loadingForm.Show();
        loadingForm.BringToFront();

        Enabled = false;

        try
        {
            nodeTable = await nodeRepository.GetByNameAsync(tableName);
            await LoadUserSettingsAsync();

            ShowSuccess("Table loaded successfully!");
        }
        catch (Exception ex)
        {
            ShowError("Error while uploading table", ex);
        }
        finally
        {
            loadingForm.Close();

            Enabled = true;
            BringToFront();
            UpdateFormTitle();
        }
    }

    private async Task DeleteTableAsync(string tableName)
    {
        using var loadingForm = new LoadingForm("Deleting table...");
        loadingForm.Show();
        loadingForm.BringToFront();

        Enabled = false;

        try
        {
            await nodeRepository.DeleteTableAsync(tableName);

            tablesToolStripMenuItem.DropDownItems.RemoveByKey(tableName);
            tabControlService.RemoveRelatedTabPages(tableName);

            await LoadTableNamesToMenuAsync();

            ShowSuccess("File deleted successfully!");
        }
        catch (Exception ex)
        {
            ShowError("Error while deleting table", ex);
        }
        finally
        {
            loadingForm.Close();

            Enabled = true;
            BringToFront();
            UpdateFormTitle();
        }
    }

    private async Task LoadUserSettingsAsync()
    {
        if (await settingsRepository.ExistsAsync(nodeTable.Name))
        {
            userSettings = await settingsRepository.GetByTableNameAsync(nodeTable.Name);
        }
        else
        {
            userSettings = userSettingsFactory.InitializeNodeTableData(nodeTable, ClusterAlgorithm.Agglomerative);
            await settingsRepository.AddAsync(userSettings);
        }

        ApplySettingsToComponents();
    }

    private void ApplySettingsToComponents()
    {
        nodeComparisonManager.UpdateSettings(userSettings);
        graphBuilder.UpdateSettings(userSettings);
        graphSaveManager.UpdateSettings(userSettings);

        if (graph != null)
            CreateGraph();

        tabControlService.UpdateDataGridViewTabPageIfOpen(nodeTable);
        tabControlService.UpdateGViewerTabPageIfOpen(graph, nodeTable.Name);
    }

    private void OpenNodeDetailsForm(string nodeName)
    {
        if (!graphBuilder.NodeDataMap.TryGetValue(nodeName, out var nodeData))
            return;

        var detailsForm = new NodeDetailsForm(nodeData, OpenNodeDetailsForm);
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
