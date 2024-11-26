using VisualizationSystem.Models.Storages;
using VisualizationSystem.UI.Forms;

namespace VisualizationSystem.Services.UI;
public class FileDialogService
{
    private const string FileDialogTitle = "Select an Excel File";
    private const string InitialDirectory = "D:\\";
    private const string ExcelFilterPattern
        = "Excel Workbook(*.xlsx)|*.xlsx|Excel 97- Excel 2003 Workbook(*.xls)|*.xls";

    public OpenFileDialog ExcelOpenFileDialog { get; }

    public FileDialogService()
    {
        ExcelOpenFileDialog = new OpenFileDialog();

        InitializeFileDialogParameters();
    }

    public bool TryOpenFileDialog(out string filePath)
    {
        filePath = string.Empty;

        if (ExcelOpenFileDialog.ShowDialog() != DialogResult.OK)
            return false;

        filePath = ExcelOpenFileDialog.FileName;

        return true;
    }

    public bool TrySelectNameColumn(out ListSelectionResult selectedColumnResult, List<string> items, string tableName)
    {
        selectedColumnResult = new ListSelectionResult();

        using (var selectionForm = new ListSelectionForm($"name column at table {tableName}", items))
        {
            if (selectionForm.ShowDialog() != DialogResult.OK)
                return false;

            selectedColumnResult = selectionForm.SelectedItem;
        }

        return true;
    }

    private void InitializeFileDialogParameters()
    {
        ExcelOpenFileDialog.Title = FileDialogTitle;
        ExcelOpenFileDialog.InitialDirectory = InitialDirectory;
        ExcelOpenFileDialog.Filter = ExcelFilterPattern;
        ExcelOpenFileDialog.RestoreDirectory = true;
    }
}
