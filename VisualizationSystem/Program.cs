using DotNetGraph.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisualizationSystem.Models.Domain.Graphs;
using VisualizationSystem.Services.DAL;
using VisualizationSystem.Services.UI;
using VisualizationSystem.Services.Utilities.Clusterers;
using VisualizationSystem.Services.Utilities.Comparers;
using VisualizationSystem.Services.Utilities.DistanceCalculators;
using VisualizationSystem.Services.Utilities.Factories;
using VisualizationSystem.Services.Utilities.FileSystem;
using VisualizationSystem.Services.Utilities.FileSystem.ExcelHandlers;
using VisualizationSystem.Services.Utilities.Graph.Builders;
using VisualizationSystem.Services.Utilities.Graph.Helpers;
using VisualizationSystem.Services.Utilities.Graph.Managers;
using VisualizationSystem.Services.Utilities.Mappers;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Services.Utilities.Settings;
using VisualizationSystem.UI.Forms;

namespace VisualizationSystem;

internal static class Program
{
    public static IConfiguration Configuration { get; private set; } = default!;

    [STAThread]
    static void Main()
    {
        ConfigureApplication();
        InitializeConfiguration();

        var services = new ServiceCollection();
        ConfigureServices(services);

        using (var serviceProvider = services.BuildServiceProvider())
        {
            var form = serviceProvider.GetRequiredService<MainForm>();
            Application.Run(form);
        }
    }

    private static void ConfigureApplication()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
    }

    private static void InitializeConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<VisualizationSystemDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
        );

        services.AddRepositories()
            .AddUIServices()
            .AddDataProcessingServices()
            .AddClusteringServices()
            .AddGraphServices();

        services.AddTransient<MainForm>();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services.AddScoped<NodeTableRepository>()
            .AddScoped<UserSettingsRepository>()
            .AddScoped<NormalizedNodeRepository>();
    }

    private static IServiceCollection AddUIServices(this IServiceCollection services)
    {
        return services.AddSingleton<DialogManager>()
            .AddSingleton<FileWriter>()
            .AddSingleton<UserSettingsManager>()
            .AddSingleton<ISettingsSubject>(sp => sp.GetRequiredService<UserSettingsManager>())
            .AddSingleton<ISettingsManager>(sp => sp.GetRequiredService<UserSettingsManager>());
    }

    private static IServiceCollection AddDataProcessingServices(this IServiceCollection services)
    {
        return services.AddSingleton<ExcelReader>()
            .AddSingleton<NodeTableMapper>()
            .AddSingleton<ExcelDataImporter>()
            .AddSingleton<DataNormalizer>()
            .AddSingleton<MetricDistanceCalculator>()
            .AddSingleton<SimilarityComparer>()
            .AddSingleton<ICompare, DefaultComparer>();
    }

    private static IServiceCollection AddClusteringServices(this IServiceCollection services)
    {
        return services.AddSingleton<KMeansClusterer>()
            .AddSingleton<AgglomerativeClusterer>()
            .AddSingleton<DBSCANClusterer>()
            .AddSingleton<ClustererFactory>();
    }

    private static IServiceCollection AddGraphServices(this IServiceCollection services)
    {
        return services.AddSingleton<GraphColorAssigner>()
            .AddSingleton<GraphCreationManager<ExtendedGraph>>()
            .AddSingleton<GraphSaveManager>()
            .AddSingleton<IGraphBuilder<ExtendedGraph>, MsaglGraphBuilder>()
            .AddSingleton<IGraphBuilder<DotGraph>, DotNetGraphBuilder>();
    }
}