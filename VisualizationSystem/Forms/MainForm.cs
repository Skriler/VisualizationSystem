using VisualizationSystem.Models.Entities;
using VisualizationSystem.SL;

namespace VisualizationSystem.Forms
{
    public partial class MainForm : Form
    {
        private static readonly string FileDialogTitle = "Select an Excel File";
        private static readonly string InitialDirectory = "D:\\";
        private static readonly string ExcelFilterPattern
            = "Excel Workbook(*.xlsx)|*.xlsx|Excel 97- Excel 2003 Workbook(*.xls)|*.xls";

        private List<NodeObject> nodeObjects = new List<NodeObject>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenExcelFile();
        }

        private void OpenExcelFile()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                InitializeFileDialogParameters(openFileDialog);

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                string filePath = openFileDialog.FileName;

                try
                {
                    nodeObjects = ExcelReader.ReadFile(filePath);
                    MessageBox.Show("Data read successfully!", "Success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InitializeFileDialogParameters(OpenFileDialog openFileDialog)
        {
            openFileDialog.Title = FileDialogTitle;
            openFileDialog.InitialDirectory = InitialDirectory;
            openFileDialog.Filter = ExcelFilterPattern;
            openFileDialog.RestoreDirectory = true;
        }
    }
}
