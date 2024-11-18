using VisualizationSystem.Models.Entities;
using VisualizationSystem.Services.Utilities;

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

        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            InitializeFileDialogParameters(openFileDialog);

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return false;

            nodeTable = ExcelReader.ReadFile(openFileDialog.FileName);
        }

        return true;
    }

    private void InitializeFileDialogParameters(OpenFileDialog openFileDialog)
    {
        openFileDialog.Title = FileDialogTitle;
        openFileDialog.InitialDirectory = InitialDirectory;
        openFileDialog.Filter = ExcelFilterPattern;
        openFileDialog.RestoreDirectory = true;
    }
}
