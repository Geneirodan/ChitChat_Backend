using System.Reflection;
using FluentValidation;
using Identity.Emails;
using Identity.Persistence;
using Identity.Persistence.Options;
using Identity.Web.Endpoints;
using Identity.Web.Options;
using Identity.Web.Options.Configurations;
using Identity.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.OpenTelemetry;
using Shared.Web;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

builder.Logging.AddOpenTelemetryLogging();

var connectionString = configuration.GetConnectionString("Database");
services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connectionString))
    .AddScoped<ApplicationContext.Initializer>();

services.AddHealthChecks()
    .AddDbContextCheck<ApplicationContext>();

services.AddSingleton<IConfigureOptions<IdentityOptions>, IdentityOptionsConfiguration>()
    .AddIdentity<User, Role>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

services
    .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true)
    .AddFluentValidationAutoValidation()
    .AddAuthenticationAndAuthorization(configuration.GetSection("Jwt"))
    .AddSwagger()
    .AddProblemDetails()
    .AddEmailRazorService()
    .Configure<ExpirationOptions>(configuration.GetSection("Expiration"))
    .Configure<EmailOptions>(configuration.GetSection("EmailSettings"))
    .Configure<AdminOptions>(configuration.GetSection("Admin"))
    .AddScoped<ITokenService, TokenService>()
    .AddTransient<IEmailSender, EmailSender>()
    .AddSharedLocalization()
    .AddSharedOpenTelemetry(configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    await scope.ServiceProvider.GetRequiredService<ApplicationContext.Initializer>().SeedAsync();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger().UseSwaggerUI();
}

app.UseExceptionHandler()
    .UseStatusCodePages()
    .UseAuthentication()
    .UseAuthorization();

app.UseRequestLocalization();

app.MapGroup("api/v1").MapIdentityEndpoints();

app.MapHealthChecks();

await app.RunAsync();
namespace Identity.Web
{
    public partial class Program;
}