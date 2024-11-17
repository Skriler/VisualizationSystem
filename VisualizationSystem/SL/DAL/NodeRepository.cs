using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.SL.DAL;

public class NodeRepository
{
    private readonly VisualizationSystemDbContext db;

    public NodeRepository(VisualizationSystemDbContext context)
    {
        db = context;
    }

    public async Task AddTableAsync(NodeTable nodeTable)
    {
        if (nodeTable == null)
            throw new ArgumentNullException("Table must be initialized.");

        if (await ExistsAsync(nodeTable.Name))
            throw new InvalidOperationException($"Table with name '{nodeTable.Name}' already exists.");

        db.NodeTable.Add(nodeTable);
        await db.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName)) 
            throw new ArgumentException("Table name cannot be null or whitespace.", tableName);

        return await db.NodeTable.AnyAsync(table => table.Name == tableName);
    }

    public async Task<NodeTable> GetByNameAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName)) 
            throw new ArgumentException("Table name cannot be null or whitespace.", tableName);

        return await db.NodeTable
            .Include(table => table.ParameterTypes)
            .Include(table => table.NodeObjects)
            .ThenInclude(node => node.Parameters)
            .Where(table => table.Name == tableName)
            .FirstAsync();
    }

    public async Task<List<NodeTable>> GetAllAsync()
    {
        return await db.NodeTable.ToListAsync();
    }
}
