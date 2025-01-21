using Microsoft.EntityFrameworkCore;
using VisualizationSystem.Models.Entities.Settings;
using VisualizationSystem.Services.DAL.Validators;

namespace VisualizationSystem.Services.DAL.Repositories;

public class UserSettingsRepository
{
    private readonly VisualizationSystemDbContext db;
    private readonly UserSettingsValidator validator;

    public UserSettingsRepository(
        VisualizationSystemDbContext context,
        UserSettingsValidator validator
        )
    {
        db = context;
        this.validator = validator;
    }

    public async Task AddAsync(UserSettings userSettings)
    {
        await validator.ValidateForAddAsync(userSettings);

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
        await validator.ValidateForGetByTableNameAsync(tableName);

        return await db.UserSettings
            .AsNoTracking()
            .Where(settings => settings.NodeTable.Name == tableName)
            .Include(settings => settings.NodeTable)
            .Include(settings => settings.AlgorithmSettings)
            .Include(settings => settings.ParameterStates)
                .ThenInclude(state => state.ParameterType)
            .AsSplitQuery()
            .FirstAsync();
    }

    public async Task<bool> ExistsAsync(string tableName) =>
        await db.UserSettings.AnyAsync(settings => settings.NodeTable.Name == tableName);
}
