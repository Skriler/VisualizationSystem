using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities.Nodes;
using VisualizationSystem.Models.Entities.Nodes.Normalized;

namespace VisualizationSystem.Services.DAL;

public class NormalizedNodeRepository
{
    private readonly VisualizationSystemDbContext db;

    public NormalizedNodeRepository(VisualizationSystemDbContext context)
    {
        db = context;
    }

    public async Task AddNormalizedNodesAsync(List<NormalizedNode> normalizedNodes)
    {
        if (normalizedNodes == null || normalizedNodes.Count == 0)
            throw new ArgumentNullException("Normalized nodes list must be initialized and cannot be empty.");

        await db.NormalizedNodes.AddRangeAsync(normalizedNodes);
        await db.SaveChangesAsync();
    }

    public async Task<List<NormalizedNode>> GetNormalizedNodesByNodeTableIdAsync(string nodeTableName)
    {
        if (string.IsNullOrWhiteSpace(nodeTableName))
            throw new ArgumentException("Node table name cannot be null or whitespace.", nodeTableName);

        return await db.NormalizedNodes
            .Where(nn => nn.NodeTable.Name == nodeTableName)
            .Include(nn => nn.NormalizedParameters)
            .Include(nn => nn.NodeTable)
            .ToListAsync();
    }
}
