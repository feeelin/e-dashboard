using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tulahack.API.Swagger;

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Policy names map to scopes
        IEnumerable<string?> requiredScopes = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Select(attr => attr.Policy)
            .Distinct()
            .ToList();

        if (!requiredScopes.Any())
        {
            return;
        }

        var bearerScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        };

        operation.Security = [new OpenApiSecurityRequirement { [bearerScheme] = requiredScopes.ToList() }];
    }
}
