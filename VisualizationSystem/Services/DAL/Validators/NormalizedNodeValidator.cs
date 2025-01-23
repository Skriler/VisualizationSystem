using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Normalized;

namespace VisualizationSystem.Services.DAL.Validators;

public class NormalizedNodeValidator
{
    private readonly VisualizationSystemDbContext db;

    public NormalizedNodeValidator(VisualizationSystemDbContext context)
    {
        db = context;
    }

    public void ValidateForNormalizedParameterStates(List<NormalizedParameterState> normalizedParameterStates)
    {
        ArgumentNullException.ThrowIfNull(normalizedParameterStates);
    }

    public void ValidateForAddNormalizedParameters(List<NodeObject> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);
    }

    public async Task ValidateForGetNodesByTableIdAsync(int tableId)
    {
        if (tableId <= 0)
            throw new ArgumentException("Table does not exist or is invalid.");

        if (!await ExistsAsync(tableId))
            throw new InvalidOperationException($"Normalized nodes for table does not exist.");
    }

    private async Task<bool> ExistsAsync(int tableId)
    {
        return await db.NodeObjects
            .Where(no => no.NodeTableId == tableId)
            .AnyAsync(no => no.NormalizedParameters.Any());
    }
}
