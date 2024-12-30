using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities.Nodes;

namespace VisualizationSystem.Services.DAL;

public class NormalizedNodeRepository
{
    private readonly VisualizationSystemDbContext db;

    public NormalizedNodeRepository(VisualizationSystemDbContext context)
    {
        db = context;
    }

    public async Task AddNormalizedParameterStateListAsync(List<NormalizedParameterState> normalizedParameterStates)
    {
        if (normalizedParameterStates == null || normalizedParameterStates.Count == 0)
            throw new ArgumentNullException("Normalized parameter states list must be initialized and cannot be empty.");

        db.NormalizedParameterStates.AddRange(normalizedParameterStates);
        await db.SaveChangesAsync();
    }

    public async Task AddAllNormalizedParametersAsync(List<NodeObject> nodes)
    {
        if (nodes == null || nodes.Count == 0)
            throw new ArgumentNullException("Normalized nodes list must be initialized and cannot be empty.");

        var allParameters = nodes.SelectMany(n => n.NormalizedParameters).ToList();

        await db.NormalizedParameters.AddRangeAsync(allParameters);
        await db.SaveChangesAsync();
    }

    public async Task<List<NodeObject>> GetByTableNameAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Node table name cannot be null or whitespace.", tableName);

        if (!await ExistsAsync(tableName))
            throw new InvalidOperationException($"Normalized nodes for table {tableName} does not exist.");

        return await db.NodeObjects
            .Where(no => no.NodeTable.Name == tableName)
            .Include(nn => nn.NormalizedParameters)
            .ThenInclude(np => np.NormalizedParameterState)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be null or whitespace.", tableName);

        return await db.NodeObjects
            .Where(no => no.NodeTable.Name == tableName)
            .AnyAsync(no => no.NormalizedParameters.Any());
    }
}
