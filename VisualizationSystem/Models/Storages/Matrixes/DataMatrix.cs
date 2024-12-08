namespace VisualizationSystem.Models.Storages.Matrixes;

public class DataMatrix
{
    public int Rows { get; private set; }
    public int Columns { get; private set; }
    public double[,] Matrix { get; private set; }

    public DataMatrix(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Matrix = new double[rows, columns];
    }

    public void SetValue(int row, int column, double value) => Matrix[row, column] = value;

    public double GetValue(int row, int column) => Matrix[row, column];
}
