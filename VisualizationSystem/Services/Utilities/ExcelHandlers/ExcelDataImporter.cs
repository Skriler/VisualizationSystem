using System.Data;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Services.UI;
using VisualizationSystem.Services.Utilities.Mappers;

namespace VisualizationSystem.Services.Utilities.ExcelHandlers;

public class ExcelDataImporter
{
    private readonly DialogService dialogService;
    private readonly ExcelReader excelReader;
    private readonly NodeTableMapper nodeTableMapper;

    public ExcelDataImporter(DialogService dialogService, ExcelReader excelReader, NodeTableMapper nodeTableMapper)
    {
        this.dialogService = dialogService;
        this.excelReader = excelReader;
        this.nodeTableMapper = nodeTableMapper;
    }

    public bool TryReadNodeTableFromExcelFile(out List<NodeTable> nodeTables)
    {
        nodeTables = new List<NodeTable>();

        if (!dialogService.TryOpenExcelFileDialog(out var filePath))
            return false;

        var tables = excelReader.GetExcelTables(filePath);

        foreach (var table in tables)
        {
            if (!TryParseExcelTable(table, out var nodeTable))
                continue;

            nodeTables.Add(nodeTable);
        }

        return nodeTables.Any();
    }

    private bool TryParseExcelTable(DataTable dataTable, out NodeTable nodeTable)
    {
        nodeTable = new NodeTable();

        try
        {
            var columnHeaders = nodeTableMapper.GetColumnHeaders(dataTable);

            if (!dialogService.TryOpenNameColumnSelectionForm(
                    out var selectedNameColumn,
                    columnHeaders,
                    dataTable.TableName)
                )
                return false;

            nodeTable = nodeTableMapper.MapToNodeTable(dataTable, selectedNameColumn.SelectedIndex);
            return true;
        }
        catch (Exception e)
        {
            MessageBox.Show($"Error while parsing data: {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return false;
    }


}
