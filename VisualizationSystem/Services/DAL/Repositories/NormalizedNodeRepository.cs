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

    public async Task<List<NodeObject>> GetNodesByTableIdAsync(int tableId)
    {
        await validator.ValidateForGetNodesByTableIdAsync(tableId);

        return await db.NodeObjects
            .AsNoTracking()
            .Where(no => no.NodeTableId == tableId)
            .Include(nn => nn.NormalizedParameters)
                .ThenInclude(np => np.NormalizedParameterState)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int tableId) =>
        await db.NodeObjects.AnyAsync(no => no.NodeTableId == tableId && no.NormalizedParameters.Any());
}
