﻿using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities.Nodes.Normalized;

namespace VisualizationSystem.Services.DAL;

public class NormalizedNodeRepository
{
    private readonly VisualizationSystemDbContext db;

    public NormalizedNodeRepository(VisualizationSystemDbContext context)
    {
        db = context;
    }

    public async Task AddRangeAsync(List<NormNode> normalizedNodes)
    {
        if (normalizedNodes == null || normalizedNodes.Count == 0)
            throw new ArgumentNullException("Normalized nodes list must be initialized and cannot be empty.");

        db.NormNodes.AddRange(normalizedNodes);
        await db.SaveChangesAsync();
    }

    public async Task<List<NormNode>> GetByTableNameAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Node table name cannot be null or whitespace.", tableName);

        if (!await ExistsAsync(tableName))
            throw new InvalidOperationException($"Normalized nodes for table {tableName} does not exist.");

        return await db.NormNodes
            .Where(nn => nn.NodeTable.Name == tableName)
            .Include(nn => nn.NormParameters)
            .Include(nn => nn.NodeTable)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be null or whitespace.", tableName);

        return await db.NormNodes
            .AnyAsync(nn => nn.NodeTable.Name == tableName);
    }
}
