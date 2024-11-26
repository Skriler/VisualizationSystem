using System.Data;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.Services.Utilities;
using VisualizationSystem.UI.Forms;

namespace VisualizationSystem.Services.UI;

public class FileService
{
    private readonly FileDialogTitle _dialogService;
    private readonly NodeTableMapper _nodeTableMapper;
    private readonly ExcelReader _excelReaderService;

    public FileService(FileDialogTitle fileDialogService, NodeTableMapper nodeTableMapper, ExcelReader excelReaderService)
    {
        _dialogService = fileDialogService;
        _nodeTableMapper = nodeTableMapper;
        _excelReaderService = excelReaderService;
    }

    public bool TryReadNodeTableFromExcelFile(out List<NodeTable> nodeTables)
    {
        nodeTables = new List<NodeTable>();

        if (!_dialogService.TryOpenFileDialog(out var filePath))
            return false;

        var tables = _excelReaderService.GetExcelTables(filePath);

        foreach (var table in tables)
        {
            if (!TryParseExcelTable(table, out var nodeTable))
                continue;

            nodeTables.Add(nodeTable);
        }

        return true;
    }

    private bool TryParseExcelTable(DataTable dataTable, out NodeTable nodeTable)
    {
        nodeTable = new NodeTable();

        try
        {
            var columnHeaders = _nodeTableMapper.GetColumnHeaders(dataTable);

            if (!_dialogService.TryGetSelectedNameColumn(out var selectedNameColumn, columnHeaders, dataTable.TableName))
                return false;

            nodeTable = _nodeTableMapper.MapToNodeTable(dataTable, selectedNameColumn.SelectedIndex);
            return true;
        }
        catch (Exception e)
        {
            MessageBox.Show($"Error while parsing data: {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return false;
    }

    
}
