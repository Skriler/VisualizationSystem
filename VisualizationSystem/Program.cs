using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisualizationSystem.Services.DAL;
using VisualizationSystem.Services.UI;
using VisualizationSystem.Services.Utilities;
using VisualizationSystem.Services.Utilities.Comparers;
using VisualizationSystem.UI.Forms;

namespace VisualizationSystem;

internal static class Program
{
    public static IConfiguration Configuration { get; private set; } = default!;

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();

        var services = new ServiceCollection();
        ConfigureServices(services);

        using (var serviceProvider = services.BuildServiceProvider())
        {
            var form = serviceProvider.GetRequiredService<MainForm>();
            Application.Run(form);
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<VisualizationSystemDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
        );

        services.AddScoped<NodeTableRepository>();
        services.AddScoped<UserSettingsRepository>();

        services.AddSingleton<DialogService>();
        services.AddSingleton<ExcelReader>();
        services.AddSingleton<NodeTableMapper>();
        services.AddSingleton<ExcelDataImporter>();
        services.AddSingleton<NodeComparer>();
        services.AddSingleton<GraphBuilder>();

        services.AddSingleton<ICompare, DefaultComparer>();

        services.AddTransient<MainForm>();
    }
}