using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Api.Data;

namespace VideoGameCatalogue.Api.Implementation.Startup;

public static class WebApplicationExtensions
{
    public static WebApplication UseCatalogueApi(this WebApplication app, string corsPolicyName)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages(async statusCodeContext =>
        {
            var httpContext = statusCodeContext.HttpContext;
            var statusCode = httpContext.Response.StatusCode;

            if (statusCode < StatusCodes.Status400BadRequest)
            {
                return;
            }

            var title = statusCode switch
            {
                StatusCodes.Status404NotFound => "Resource not found.",
                StatusCodes.Status405MethodNotAllowed => "Method not allowed.",
                StatusCodes.Status401Unauthorized => "Unauthorized.",
                StatusCodes.Status403Forbidden => "Forbidden.",
                _ => "Request could not be completed."
            };

            var detail = statusCode switch
            {
                StatusCodes.Status404NotFound => "The requested resource was not found.",
                StatusCodes.Status405MethodNotAllowed => "The requested HTTP method is not supported for this endpoint.",
                StatusCodes.Status401Unauthorized => "The request requires authentication.",
                StatusCodes.Status403Forbidden => "The request was understood but is not permitted.",
                _ => "The request could not be completed."
            };

            var result = Results.Problem(
                statusCode: statusCode,
                title: title,
                detail: detail,
                instance: httpContext.Request.Path,
                extensions: new Dictionary<string, object?>
                {
                    ["traceId"] = httpContext.TraceIdentifier
                });

            await result.ExecuteAsync(httpContext);
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(corsPolicyName);
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    public static async Task EnsureCatalogueDatabaseAsync(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("CatalogueDatabaseInitialization");

        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogueDbContext>();

        try
        {
            if (dbContext.Database.IsRelational())
            {
                // Migration auto-application is used here for simplicity and demonstration purposes. 
                // In production scenarios, consider using explicit migration application strategies to have more control over the deployment process and handle potential issues that may arise during schema changes.
                logger.LogInformation("Applying catalogue database migrations.");
                await dbContext.Database.MigrateAsync();
            }
            else
            {
                logger.LogInformation("Ensuring non-relational catalogue database is created.");
                await dbContext.Database.EnsureCreatedAsync();
            }

            logger.LogInformation("Catalogue database initialization completed successfully.");
        }
        catch (Exception exception)
        {
            logger.LogCritical(exception, "Catalogue database initialization failed.");
            throw;
        }
    }
}