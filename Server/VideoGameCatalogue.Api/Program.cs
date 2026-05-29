using VideoGameCatalogue.Api.Implementation.Startup;

namespace VideoGameCatalogue.Api;

public partial class Program
{
    private const string FrontendCorsPolicy = "FrontendCorsPolicy";

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        await ConfigureApplicationAsync(app);
        await app.RunAsync();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCatalogueApi(configuration, FrontendCorsPolicy);
    }

    private static async Task ConfigureApplicationAsync(WebApplication app)
    {
        app.UseCatalogueApi(FrontendCorsPolicy);
        await app.EnsureCatalogueDatabaseAsync();
    }
}
