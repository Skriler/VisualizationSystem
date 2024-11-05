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
        //NormalizeParameterTypes(nodeTable.ParameterTypes);

        //foreach (NodeObject node in nodeTable.NodeObjects)
        //{
        //    NormalizeNode(node);
        //}

        db.NodeTable.Add(nodeTable);

        await db.SaveChangesAsync();
    }

    public async Task<NodeTable> GetTableAsync()
    {
        // TODO
        if (!db.NodeTable.Any())
            return new NodeTable();

        return await db.NodeTable
            .Include(table => table.ParameterTypes)
            .Include(table => table.NodeObjects)
            .ThenInclude(node => node.Parameters)
            .FirstAsync();
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
