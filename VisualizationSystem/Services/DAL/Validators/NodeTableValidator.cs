using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Services.DAL.Validators;

public class NodeTableValidator
{
    private readonly VisualizationSystemDbContext db;

    public NodeTableValidator(VisualizationSystemDbContext context)
    {
        db = context;
    }

    public async Task ValidateForAddAsync(NodeTable nodeTable)
    {
        ArgumentNullException.ThrowIfNull(nodeTable);

        if (await ExistsAsync(nodeTable.Name))
            throw new InvalidOperationException($"Table with name '{nodeTable.Name}' already exists.");
    }

    public async Task ValidateForAddRangeAsync(List<NodeTable> nodeTables)
    {
        ArgumentNullException.ThrowIfNull(nodeTables);

        var tableNames = nodeTables.ConvertAll(nt => nt.Name);
        var duplicateTableNames = await db.NodeTables
            .Where(table => tableNames.Contains(table.Name))
            .Select(table => table.Name)
            .ToListAsync();

        if (duplicateTableNames.Count != 0)
            throw new InvalidOperationException(
                $"Tables with the following names already exist: {string.Join(", ", duplicateTableNames)}"
                );
    }

    public async Task ValidateForDeleteAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be null or whitespace.", tableName);

        if (!await ExistsAsync(tableName))
            throw new InvalidOperationException($"Table with name '{tableName}' does not exist.");
    }

    public async Task ValidateForGetByNameAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be null or whitespace.", tableName);

        if (!await ExistsAsync(tableName))
            throw new InvalidOperationException($"Table with name '{tableName}' does not exist.");
    }

    private async Task<bool> ExistsAsync(string tableName)
        => await db.NodeTables.AnyAsync(table => table.Name == tableName);
}
