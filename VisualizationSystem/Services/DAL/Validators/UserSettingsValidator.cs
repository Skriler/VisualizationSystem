using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities.Settings;

namespace VisualizationSystem.Services.DAL.Validators;

public class UserSettingsValidator
{
    private readonly VisualizationSystemDbContext db;

    public UserSettingsValidator(VisualizationSystemDbContext context)
    {
        db = context;
    }

    public async Task ValidateForAddAsync(UserSettings userSettings)
    {
        if (await ExistsAsync(userSettings.NodeTable.Name))
            throw new InvalidOperationException($"Settings for table {userSettings.NodeTable.Name} already exists.");
    }

    public async Task ValidateForGetByTableNameAsync(string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be null or whitespace.", tableName);

        if (!await ExistsAsync(tableName))
            throw new InvalidOperationException($"Settings for table {tableName} does not exist.");
    }

    private async Task<bool> ExistsAsync(string tableName)
        => await db.UserSettings.AnyAsync(settings => settings.NodeTable.Name == tableName);
}
