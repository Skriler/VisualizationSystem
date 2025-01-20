using DotNetGraph.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisualizationSystem.Models.Domain.Graphs;
using VisualizationSystem.Services.DAL;
using VisualizationSystem.Services.DAL.Repositories;
using VisualizationSystem.Services.DAL.Validators;
using VisualizationSystem.Services.UI;
using VisualizationSystem.Services.UI.TabPages;
using VisualizationSystem.Services.Utilities.Clusterers;
using VisualizationSystem.Services.Utilities.Comparers;
using VisualizationSystem.Services.Utilities.DimensionReducers;
using VisualizationSystem.Services.Utilities.DistanceCalculators;
using VisualizationSystem.Services.Utilities.DistanceCalculators.CategoricalMetrics;
using VisualizationSystem.Services.Utilities.DistanceCalculators.NumericMetrics;
using VisualizationSystem.Services.Utilities.Factories;
using VisualizationSystem.Services.Utilities.FileSystem;
using VisualizationSystem.Services.Utilities.FileSystem.ExcelHandlers;
using VisualizationSystem.Services.Utilities.Graphs.Builders;
using VisualizationSystem.Services.Utilities.Graphs.Managers;
using VisualizationSystem.Services.Utilities.Helpers.Colors;
using VisualizationSystem.Services.Utilities.Mappers;
using VisualizationSystem.Services.Utilities.Normalizers;
using VisualizationSystem.Services.Utilities.Plots;
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
            .AddDistanceServices()
            .AddClusteringServices()
            .AddGraphServices()
            .AddPlotServices();

        services.AddTransient<MainForm>();
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        return services
            .AddScoped<NodeTableValidator>()
            .AddScoped<UserSettingsValidator>()
            .AddScoped<NormalizedNodeValidator>();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<NodeTableRepository>()
            .AddScoped<UserSettingsRepository>()
            .AddScoped<NormalizedNodeRepository>();
    }

    private static IServiceCollection AddUIServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<DialogManager>()
            .AddSingleton<UserSettingsManager>()
            .AddSingleton<TabControlService>()
            .AddSingleton<ISettingsSubject>(sp => sp.GetRequiredService<UserSettingsManager>())
            .AddSingleton<ISettingsManager>(sp => sp.GetRequiredService<UserSettingsManager>())
            .AddTransient<ITabStrategy, DataGridViewTabStrategy>()
            .AddTransient<ITabStrategy, GViewerTabStrategy>()
            .AddTransient<ITabStrategy, ClusteredPlotTabStrategy>()
            .AddScoped<FileWriter>();
    }

    private static IServiceCollection AddDataProcessingServices(this IServiceCollection services)
    {
        return services
            .AddTransient<ExcelReader>()
            .AddTransient<NodeTableMapper>()
            .AddTransient<DataPointMapper>()
            .AddTransient<ExcelDataImporter>()
            .AddTransient<DataNormalizer>()
            .AddTransient<SimilarityComparer>()
            .AddSingleton<ICompare, DefaultComparer>()
            .AddSingleton<IDimensionReducer, PCAReducer>();
    }

    private static IServiceCollection AddDistanceServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<EuclideanDistanceMetric>()
            .AddSingleton<ManhattanDistanceMetric>()
            .AddSingleton<CosineDistanceMetric>()
            .AddSingleton<HammingDistanceMetric>()
            .AddSingleton<IDistanceCalculator, DistanceCalculator>()
            .AddTransient<DistanceCalculatorFactory>();
    }

    private static IServiceCollection AddClusteringServices(this IServiceCollection services)
    {
        return services
            .AddTransient<KMeansClusterer>()
            .AddTransient<AgglomerativeClusterer>()
            .AddTransient<DBSCANClusterer>()
            .AddTransient<ClustererFactory>();
    }

    private static IServiceCollection AddGraphServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<ColorHelper>()
            .AddScoped<GraphCreationManager<ExtendedGraph>>()
            .AddScoped<GraphSaveManager>()
            .AddTransient<IGraphBuilder<ExtendedGraph>, MsaglGraphBuilder>()
            .AddTransient<IGraphBuilder<DotGraph>, DotNetGraphBuilder>();
    }

    private static IServiceCollection AddPlotServices(this IServiceCollection services)
    {
        return services
            .AddScoped<PlotCreationManager>()
            .AddTransient<PlotConfigurator>();
    }
}