using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Storages;
using VisualizationSystem.Services.Utilities;
using VisualizationSystem.UI.Forms;

namespace VisualizationSystem.Services.UI;

public class FileService
{
    private const string FileDialogTitle = "Select an Excel File";
    private const string InitialDirectory = "D:\\";
    private const string ExcelFilterPattern
        = "Excel Workbook(*.xlsx)|*.xlsx|Excel 97- Excel 2003 Workbook(*.xls)|*.xls";

    public bool TryReadNodeTableFromExcelFile(out NodeTable nodeTable)
    {
        nodeTable = new NodeTable();

        if (!TryGetExcelFilePath(out var filePath))
            return false;

        var columnHeaders = ExcelReader.GetColumnHeaders(filePath);
        
        if (!TryGetSelectedNameColumn(out var selectedNameColumn, columnHeaders))
            return false;

        nodeTable = ExcelReader.ReadFile(filePath, selectedNameColumn.SelectedIndex);
        return true;
    }

    private bool TryGetExcelFilePath(out string filePath)
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

    private bool TryGetSelectedNameColumn(out ListSelectionResult columnSelectionResult, List<string> items)
    {
        columnSelectionResult = new ListSelectionResult();

        using (var listSelectionForm = new ListSelectionForm("name column", items))
        {
            if (listSelectionForm.ShowDialog() != DialogResult.OK)
                return false;

            columnSelectionResult = listSelectionForm.SelectedItem;
        }

        return true;
    }

    private void InitializeExcelFileDialogParameters(OpenFileDialog openFileDialog)
    {
        openFileDialog.Title = FileDialogTitle;
        openFileDialog.InitialDirectory = InitialDirectory;
        openFileDialog.Filter = ExcelFilterPattern;
        openFileDialog.RestoreDirectory = true;
    }
}
