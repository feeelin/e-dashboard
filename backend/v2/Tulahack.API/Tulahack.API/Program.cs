using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tulahack.API.Context;
using Tulahack.API.Extensions;
using Tulahack.API.Static;
using Tulahack.API.Utils;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KeycloakConfiguration>(builder.Configuration.GetSection(KeycloakConfiguration.Keycloak));
builder.Services.Configure<CdnConfiguration>(builder.Configuration.GetSection(CdnConfiguration.Cdn));
builder.Services.Configure<WebConfiguration>(builder.Configuration.GetSection(WebConfiguration.Web));

builder.Services.AddDbContext<ITulahackContext, TulahackContext>(options =>
{
    _ = options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

    _ = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "appData", "storage", "database"));
    var datasource = Path.Combine(Directory.GetCurrentDirectory(), "appData", "storage", "database", "tulahack.db");
    _ = options.UseSqlite($"Data Source={datasource}");
});

builder.Services
    .AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGenNewtonsoftSupport();
builder.Services.AddCustomSwagger();

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services
    .AddAuthorization(o =>
        {
            o.AddPolicy("Public", b =>
                _ = b.RequireClaim(TulahackClaimTypes.KeycloakGroup, Groups.Public));
            o.AddPolicy("Public+", b =>
                _ = b.RequireClaim(TulahackClaimTypes.KeycloakGroup, Groups.PublicPlus));

            o.AddPolicy("Manager", b =>
                _ = b.RequireClaim(TulahackClaimTypes.KeycloakGroup, Groups.Managers));
            o.AddPolicy("Manager+", b =>
                _ = b.RequireClaim(TulahackClaimTypes.KeycloakGroup, Groups.ManagerPlus));

            o.AddPolicy("Superuser", b =>
                _ = b.RequireClaim(TulahackClaimTypes.KeycloakGroup, Groups.Superuser));
        }
    ).AddKeycloakAuthorization(builder.Configuration);

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<TulahackProfile>());

builder.Services.AddServices();

WebApplication app = builder.Build();

app.UseSwagger(c => { c.RouteTemplate = "api/swagger/{documentName}/swagger.json"; });
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Tulahack API V2");
    c.RoutePrefix = "api/swagger";
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "appData", "storage")),
    RequestPath = "/api/storage",
    // Uncomment to enable static files authorization
    // OnPrepareResponse = ctx =>
    // {
    //     if (ctx.Context.User.Identity is { IsAuthenticated: true }) return;
    //
    //     // respond HTTP 401 Unauthorized.
    //     ctx.Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    //     // flush response
    //     ctx.Context.Response.ContentLength = 0;
    //     ctx.Context.Response.Body = Stream.Null;
    //     // disable cache
    //     ctx.Context.Response.Headers.Append("Cache-Control", "no-store");
    // }
});

// Initial migration and DB seeding
using (IServiceScope scope = app.Services.CreateScope())
{
    if (scope.ServiceProvider.GetRequiredService<ITulahackContext>() is TulahackContext context)
    {
        IEnumerable<string> migrations = await context.Database.GetPendingMigrationsAsync();

        if (migrations.Any())
        {
            await context.Database.MigrateAsync();
        }
    }
}

await app.RunAsync();