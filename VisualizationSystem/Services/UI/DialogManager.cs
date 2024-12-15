using VisualizationSystem.Models.Storages.Results;
using VisualizationSystem.UI.Forms;

namespace VisualizationSystem.Services.UI;

public class DialogManager
{
    private const string FileDialogTitle = "Select an Excel File";
    private const string InitialDirectory = "D:\\";
    private const string ExcelFilterPattern
        = "Excel Workbook(*.xlsx)|*.xlsx|Excel 97- Excel 2003 Workbook(*.xls)|*.xls";

    public bool TryOpenExcelFileDialog(out string filePath)
    {
        filePath = string.Empty;

        using (var excelOpenFileDialog = new OpenFileDialog())
        {
            ConfigureFileDialog(excelOpenFileDialog);

            if (excelOpenFileDialog.ShowDialog() != DialogResult.OK)
                return false;

            filePath = excelOpenFileDialog.FileName;
        }

        return true;
    }

    public bool TryOpenNameColumnSelectionForm(out ListSelectionResult selectedColumnResult, List<string> items, string tableName)
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

    private void ConfigureFileDialog(OpenFileDialog excelOpenFileDialog)
    {
        excelOpenFileDialog.Title = FileDialogTitle;
        excelOpenFileDialog.InitialDirectory = InitialDirectory;
        excelOpenFileDialog.Filter = ExcelFilterPattern;
        excelOpenFileDialog.RestoreDirectory = true;
    }
}
