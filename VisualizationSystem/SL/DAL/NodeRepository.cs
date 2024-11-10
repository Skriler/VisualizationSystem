using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Xml.Linq;
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

    public async Task AddTableAsync(NodeTable nodeTable)
    {
        if (nodeTable == null)
            throw new ArgumentNullException(nameof(nodeTable));

        // TODO
        if (await ExistsAsync(nodeTable.Name))
            throw new InvalidOperationException($"Table with name '{nodeTable.Name}' already exists.");

        db.NodeTable.Add(nodeTable);
        await db.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName)) 
            throw new ArgumentException("Table name cannot be null or whitespace.", nameof(tableName));

        return await db.NodeTable.AnyAsync(table => table.Name == tableName);
    }

    public async Task<NodeTable> GetByNameAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName)) 
            throw new ArgumentException("Table name cannot be null or whitespace.", nameof(tableName));

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

    //private void NormalizeParameterTypes(List<ParameterType> parameterTypes)
    //{
    //    foreach (var parameterType in parameterTypes)
    //    {
    //        parameterType.Name = NormalizeString(parameterType.Name);
    //    }
    //}

    //private void NormalizeNode(NodeObject node)
    //{
    //    node.Name = NormalizeString(node.Name);

    //    foreach (var parameter in node.Parameters)
    //    {
    //        parameter.Value = NormalizeString(parameter.Value);
    //    }
    //}

    //private string NormalizeString(string? str)
    //{
    //    if (string.IsNullOrWhiteSpace(str))
    //        return string.Empty;

    //    return Regex.Replace(str, NormalizationPattern, "_").Trim();
    //}
}
