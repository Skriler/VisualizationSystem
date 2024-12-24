using DotNetGraph.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisualizationSystem.Models.Domain.Graphs;
using VisualizationSystem.Services.DAL;
using VisualizationSystem.Services.UI;
using VisualizationSystem.Services.Utilities.Clusterers;
using VisualizationSystem.Services.Utilities.Comparers;
using VisualizationSystem.Services.Utilities.Factories;
using VisualizationSystem.Services.Utilities.FileSystem;
using VisualizationSystem.Services.Utilities.FileSystem.ExcelHandlers;
using VisualizationSystem.Services.Utilities.Graph;
using VisualizationSystem.Services.Utilities.Graph.Builders;
using VisualizationSystem.Services.Utilities.Graph.Helpers;
using VisualizationSystem.Services.Utilities.Managers;
using VisualizationSystem.Services.Utilities.Mappers;
using VisualizationSystem.Services.Utilities.Normalizers;
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
        services.AddScoped<NormalizedNodeRepository>();

        services.AddSingleton<DialogManager>();
        services.AddSingleton<ExcelReader>();
        services.AddSingleton<NodeTableMapper>();
        services.AddSingleton<ExcelDataImporter>();
        services.AddSingleton<NodeComparisonManager>();
        services.AddSingleton<GraphSaveManager>();
        services.AddSingleton<GraphColorAssigner>();
        services.AddSingleton<FileWriter>();

        services.AddSingleton<DataNormalizer>();
        services.AddSingleton<KMeansClusterer>();
        services.AddSingleton<AgglomerativeClusterer>();
        services.AddSingleton<DBSCANClusterer>();

        services.AddSingleton<ClustererFactory>();
        services.AddSingleton<UserSettingsManager>();
        services.AddSingleton<GraphCreationManager>();

        services.AddSingleton<ICompare, DefaultComparer>();
        services.AddSingleton<IGraphBuilder<ExtendedGraph>, MsaglGraphBuilder>();
        services.AddSingleton<IGraphBuilder<DotGraph>, DotNetGraphBuilder>();

        services.AddTransient<MainForm>();
    }
}