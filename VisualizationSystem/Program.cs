using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisualizationSystem.SL.DAL;
using VisualizationSystem.Forms;

namespace VisualizationSystem;

internal static class Program
{
    public static IConfiguration Configuration { get; private set; } = default!;

    [STAThread]
    static void Main()
    {
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

        services.AddTransient<MainForm>();
    }
}