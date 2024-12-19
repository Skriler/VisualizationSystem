using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Xml;
using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Services.DAL;

public class NodeTableRepository
{
    private readonly VisualizationSystemDbContext db;

    public NodeTableRepository(VisualizationSystemDbContext context)
    {
        db = context;
    }

    public async Task AddAsync(NodeTable nodeTable)
    {
        if (nodeTable == null)
            throw new ArgumentNullException("Table must be initialized.");

        if (await ExistsAsync(nodeTable.Name))
            throw new InvalidOperationException($"Table with name '{nodeTable.Name}' already exists.");

        db.NodeTables.Add(nodeTable);
        await db.SaveChangesAsync();
    }

    public async Task AddListAsync(List<NodeTable> nodeTables)
    {
        if (nodeTables == null || nodeTables.Count == 0)
            throw new ArgumentNullException("Table list must be initialized and cannot be empty.");

        foreach (var nodeTable in nodeTables)
        {
            if (await ExistsAsync(nodeTable.Name))
                throw new InvalidOperationException($"Table with name '{nodeTable.Name}' already exists.");
        }

        db.NodeTables.AddRange(nodeTables);
        await db.SaveChangesAsync();
    }

    public async Task DeleteTableAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be null or whitespace.", tableName);

        var nodeTable = await db.NodeTables
            .FirstOrDefaultAsync(table => table.Name == tableName);

        if (nodeTable == null)
            throw new InvalidOperationException($"Table with name '{tableName}' does not exist.");

        var parameterTypes = nodeTable.ParameterTypes;

        db.NodeTables.Remove(nodeTable);
        db.ParameterTypes.RemoveRange(parameterTypes);

        await db.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName)) 
            throw new ArgumentException("Table name cannot be null or whitespace.", tableName);

        return await db.NodeTables.AnyAsync(table => table.Name == tableName);
    }

    public async Task<NodeTable> GetByNameAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName)) 
            throw new ArgumentException("Table name cannot be null or whitespace.", tableName);

        return await db.NodeTables
            .Include(table => table.ParameterTypes)
            .Include(table => table.NodeObjects)
            .ThenInclude(node => node.Parameters)
            .Where(table => table.Name == tableName)
            .FirstAsync();
    }

    public async Task<List<NodeTable>> GetAllAsync()
    {
        return await db.NodeTables.ToListAsync();
    }
}
