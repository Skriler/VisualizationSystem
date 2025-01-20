using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Services.DAL.Validators;

namespace VisualizationSystem.Services.DAL.Repositories;

public class NodeTableRepository
{
    private readonly VisualizationSystemDbContext db;
    private readonly NodeTableValidator validator;

    public NodeTableRepository(
        VisualizationSystemDbContext context,
        NodeTableValidator validator
        )
    {
        db = context;
        this.validator = validator;
    }

    public async Task AddAsync(NodeTable nodeTable)
    {
        await validator.ValidateForAddAsync(nodeTable);

        db.NodeTables.Add(nodeTable);
        await db.SaveChangesAsync();
    }

    public async Task AddListAsync(List<NodeTable> nodeTables)
    {
        await validator.ValidateForAddRangeAsync(nodeTables);

        db.NodeTables.AddRange(nodeTables);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string tableName)
    {
        await validator.ValidateForDeleteAsync(tableName);

        var nodeTable = await db.NodeTables
            .Include(table => table.ParameterTypes)
            .FirstAsync(table => table.Name == tableName);
        var parameterTypes = nodeTable.ParameterTypes;

        await using var transaction = await db.Database.BeginTransactionAsync();

        db.NodeTables.Remove(nodeTable);
        await db.SaveChangesAsync();

        db.ParameterTypes.RemoveRange(nodeTable.ParameterTypes);
        await db.SaveChangesAsync();

        await transaction.CommitAsync();
    }

    public async Task<NodeTable> GetByNameAsync(string tableName)
    {
        await validator.ValidateForGetByNameAsync(tableName);

        return await db.NodeTables
            .Include(table => table.ParameterTypes)
            .Include(table => table.NodeObjects)
            .ThenInclude(node => node.Parameters)
            .Where(table => table.Name == tableName)
            .FirstAsync();
    }

    public async Task<List<NodeTable>> GetAllAsync() => await db.NodeTables.ToListAsync();
}
