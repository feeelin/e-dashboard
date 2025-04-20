using Microsoft.OpenApi.Models;
using Tulahack.API.Services;
using Tulahack.API.Swagger;

namespace Tulahack.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IDashboardService, DashboardService>();
        services.AddTransient<IStorageService, StorageService>();
        services.AddTransient<ITaskService, TaskService>();
        services.AddTransient<IContestService, ContestService>();
    }

    public static void AddCustomSwagger(this IServiceCollection services) =>
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Tulahack API",
                Version = "v2",
            });

            option.OperationFilter<SecurityRequirementsOperationFilter>();

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            };
            option.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });
        });
}
