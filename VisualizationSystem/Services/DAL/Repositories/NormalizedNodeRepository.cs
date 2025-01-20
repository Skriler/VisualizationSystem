using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Normalized;
using VisualizationSystem.Services.DAL.Validators;

namespace VisualizationSystem.Services.DAL.Repositories;

public class NormalizedNodeRepository
{
    private readonly VisualizationSystemDbContext db;
    private readonly NormalizedNodeValidator validator;

    public NormalizedNodeRepository(
        VisualizationSystemDbContext context,
        NormalizedNodeValidator validator
        )
    {
        db = context;
        this.validator = validator;
    }

    public async Task AddNormalizedParameterStatesAsync(List<NormalizedParameterState> normalizedParameterStates)
    {
        validator.ValidateForNormalizedParameterStates(normalizedParameterStates);

        db.NormalizedParameterStates.AddRange(normalizedParameterStates);
        await db.SaveChangesAsync();
    }

    public async Task AddNormalizedParametersAsync(List<NodeObject> nodes)
    {
        validator.ValidateForAddNormalizedParameters(nodes);

        var allParameters = nodes
            .SelectMany(n => n.NormalizedParameters)
            .ToList();

        await db.NormalizedParameters.AddRangeAsync(allParameters);
        await db.SaveChangesAsync();
    }

    public async Task<List<NodeObject>> GetNodesByTableNameAsync(string tableName)
    {
        await validator.ValidateForGetNodesByTableNameAsync(tableName);

        return await db.NodeObjects
            .Where(no => no.NodeTable.Name == tableName)
            .Include(nn => nn.NormalizedParameters)
            .ThenInclude(np => np.NormalizedParameterState)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(string tableName)
    {
        return await db.NodeObjects
            .Where(no => no.NodeTable.Name == tableName)
            .AnyAsync(no => no.NormalizedParameters.Any());
    }
}
