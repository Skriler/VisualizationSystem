using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities;

namespace VisualizationSystem.SL.DAL;

public class UserSettingsRepository
{
    private readonly VisualizationSystemDbContext db;

    public UserSettingsRepository(VisualizationSystemDbContext context)
    {
        db = context;
    }

    public async Task<UserSettings> GetByTableNameAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be null or whitespace.", tableName);

        return await db.UserSettings
            .Include(us => us.NodeTable)
            .Include(us => us.ParameterStates)
            .FirstOrDefaultAsync(us => us.NodeTable.Name == tableName);
    }
}
