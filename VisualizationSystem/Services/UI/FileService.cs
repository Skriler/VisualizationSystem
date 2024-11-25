using System.Data;
using System.Windows.Forms;
using System.Xml;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.Services.Utilities;
using VisualizationSystem.UI.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VisualizationSystem.Services.UI;

public static class FileService
{
    private const string FileDialogTitle = "Select an Excel File";
    private const string InitialDirectory = "D:\\";
    private const string ExcelFilterPattern
        = "Excel Workbook(*.xlsx)|*.xlsx|Excel 97- Excel 2003 Workbook(*.xls)|*.xls";

    public static bool TryReadNodeTableFromExcelFile(out List<NodeTable> nodeTables)
    {
        nodeTables = new List<NodeTable>();

        if (!TryGetExcelFilePath(out var filePath))
            return false;

        var tables = ExcelParser.GetExcelTables(filePath);

        foreach (var table in tables)
        {
            if (!TryParseExcelTable(table, out var nodeTable))
                continue;

            nodeTables.Add(nodeTable);
        }

        return true;
    }

    private static bool TryParseExcelTable(DataTable dataTable, out NodeTable nodeTable)
    {
        nodeTable = new NodeTable();

        try
        {
            var columnHeaders = ExcelParser.GetColumnHeaders(dataTable);

            if (!TryGetSelectedNameColumn(out var selectedNameColumn, columnHeaders, dataTable.TableName))
                return false;

            nodeTable = ExcelParser.ParseTable(dataTable, selectedNameColumn.SelectedIndex);
            return true;
        }
        catch (Exception e)
        {
            MessageBox.Show($"Error while parsing data: {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return false;
    }

    private static bool TryGetExcelFilePath(out string filePath)
    {
        filePath = string.Empty;

        using (var openFileDialog = new OpenFileDialog())
        {
            InitializeExcelFileDialogParameters(openFileDialog);

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return false;

            filePath = openFileDialog.FileName;
        }

        return true;
    }

    private static bool TryGetSelectedNameColumn(out ListSelectionResult columnSelectionResult, List<string> items, string tableName)
    {
        columnSelectionResult = new ListSelectionResult();

        using (var listSelectionForm = new ListSelectionForm($"name column at table {tableName}", items))
        {
            if (listSelectionForm.ShowDialog() != DialogResult.OK)
                return false;

            columnSelectionResult = listSelectionForm.SelectedItem;
        }

        return true;
    }

    private static void InitializeExcelFileDialogParameters(OpenFileDialog openFileDialog)
    {
        openFileDialog.Title = FileDialogTitle;
        openFileDialog.InitialDirectory = InitialDirectory;
        openFileDialog.Filter = ExcelFilterPattern;
        openFileDialog.RestoreDirectory = true;
    }
}
