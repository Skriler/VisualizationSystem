using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.SL.DAL;

public class NodeRepository
{
    private static readonly string NormalizationPattern = @"[ ,;]";

    private readonly VisualizationSystemDbContext db;

    public NodeRepository(VisualizationSystemDbContext context)
    {
        db = context;
    }

    public async Task AddListAsync(List<NodeObject> nodes)
    {
        foreach (NodeObject node in nodes)
        {
            NormalizeNode(node);
            db.NodeObjects.Add(node);
        }

        await db.SaveChangesAsync();
    }

    public async Task<List<NodeObject>> GetAllAsync()
    {
        return await db.NodeObjects
            .Include(node => node.Parameters)
            .ToListAsync();
    }

    private void NormalizeNode(NodeObject node)
    {
        node.Name = NormalizeString(node.Name);

        foreach (var parameter in node.Parameters)
        {
            parameter.Name = NormalizeString(parameter.Name);
            parameter.Value = NormalizeString(parameter.Value);
        }
    }

    private string NormalizeString(string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return string.Empty;

        return Regex.Replace(str, NormalizationPattern, "_").Trim();
    }
}
