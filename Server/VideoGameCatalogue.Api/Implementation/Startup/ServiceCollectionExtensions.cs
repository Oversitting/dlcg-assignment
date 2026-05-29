using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Api.Contracts;
using VideoGameCatalogue.Api.Data;
using VideoGameCatalogue.Api.Implementation.ErrorHandling;
using VideoGameCatalogue.Api.Implementation.Services;
using VideoGameCatalogue.Api.Implementation.Validators;

namespace VideoGameCatalogue.Api.Implementation.Startup;

/// <summary>
/// Registers the API's infrastructure, application services, validation, and OpenAPI support.
/// </summary>
public static class ServiceCollectionExtensions
{
    private const string CorsOriginsSectionName = "Cors:AllowedOrigins";

    /// <summary>
    /// Adds the catalogue API services and middleware dependencies to the application service collection.
    /// </summary>
    /// <param name="services">The service collection being configured.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="corsPolicyName">The CORS policy name used for the frontend origin list.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddCatalogueApi(
        this IServiceCollection services,
        IConfiguration configuration,
        string corsPolicyName)
    {
        var connectionString = configuration.GetConnectionString("CatalogueDatabase")
            ?? throw new InvalidOperationException("Connection string 'CatalogueDatabase' was not found.");
        var allowedOrigins = configuration.GetSection(CorsOriginsSectionName).Get<string[]>()
            ?? Array.Empty<string>();

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddControllers();
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<SaveVideoGameRequestValidator>();
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("RequestValidation");

                var errors = context.ModelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

                logger.LogWarning(
                    "Model validation failed for {Method} {Path}. TraceId: {TraceId}. Errors: {@Errors}",
                    context.HttpContext.Request.Method,
                    context.HttpContext.Request.Path,
                    context.HttpContext.TraceIdentifier,
                    errors);

                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Request validation failed.",
                    Detail = "One or more validation errors occurred.",
                    Instance = context.HttpContext.Request.Path
                };

                problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

                return new BadRequestObjectResult(problemDetails);
            };
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });
        services.AddCors(options =>
        {
            options.AddPolicy(corsPolicyName, policy =>
            {
                if (allowedOrigins.Length == 0)
                {
                    return;
                }

                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        services.AddDbContext<CatalogueDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IVideoGameService, VideoGameService>();

        return services;
    }
}