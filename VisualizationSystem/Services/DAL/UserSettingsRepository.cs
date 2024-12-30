using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities.Settings;

namespace VisualizationSystem.Services.DAL;

public class UserSettingsRepository
{
    private readonly VisualizationSystemDbContext db;

    public UserSettingsRepository(VisualizationSystemDbContext context)
    {
        db = context;
    }

    public async Task AddAsync(UserSettings userSettings)
    {
        if (await ExistsAsync(userSettings.NodeTable.Name))
            throw new InvalidOperationException($"Settings for table {userSettings.NodeTable.Name} already exists.");

        db.UserSettings.Add(userSettings);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserSettings userSettings)
    {
        db.UserSettings.Update(userSettings);
        await db.SaveChangesAsync();
    }

    public async Task<UserSettings> GetByTableNameAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be null or whitespace.", tableName);

        if (!await ExistsAsync(tableName))
            throw new InvalidOperationException($"Settings for table {tableName} does not exist.");

        return await db.UserSettings
            .Include(settings => settings.NodeTable)
            .Include(settings => settings.ParameterStates)
            .Include(settings => settings.AlgorithmSettings)
            .FirstAsync(settings => settings.NodeTable.Name == tableName);
    }

    public async Task<bool> ExistsAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be null or whitespace.", tableName);

        return await db.UserSettings
            .AnyAsync(settings => settings.NodeTable.Name == tableName);
    }
}
