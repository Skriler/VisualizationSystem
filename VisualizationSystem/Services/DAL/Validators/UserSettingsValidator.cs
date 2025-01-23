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
        if (await ExistsAsync(userSettings.NodeTableId))
            throw new InvalidOperationException($"Settings for table already exists.");
    }

    public async Task ValidateForGetByTableIdAsync(int tableId)
    {
        if (tableId <= 0)
            throw new ArgumentException("Table does not exist or is invalid.");

        if (!await ExistsAsync(tableId))
            throw new InvalidOperationException($"Settings for table does not exist.");
    }

    private async Task<bool> ExistsAsync(int tableId)
        => await db.UserSettings.AnyAsync(settings => settings.NodeTableId == tableId);
}
