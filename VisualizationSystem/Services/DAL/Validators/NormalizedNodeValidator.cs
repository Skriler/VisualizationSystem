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

    public async Task ValidateForGetNodesByTableNameAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Node table name cannot be null or whitespace.", tableName);

        if (!await ExistsAsync(tableName))
            throw new InvalidOperationException($"Normalized nodes for table {tableName} does not exist.");
    }

    private async Task<bool> ExistsAsync(string tableName)
    {
        return await db.NodeObjects
            .Where(no => no.NodeTable.Name == tableName)
            .AnyAsync(no => no.NormalizedParameters.Any());
    }
}
