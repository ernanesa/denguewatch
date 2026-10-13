using api.Configuration;
using api.Data.Documents;
using api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<MongoSettings>()
                .Bind(builder.Configuration.GetSection(MongoSettings.SectionName))
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.ConnectionString), "MongoSettings:ConnectionString must be provided.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.DatabaseName), "MongoSettings:DatabaseName must be provided.")
                .ValidateOnStart();

builder.Services.AddOptions<DemoUserSettings>()
                .Bind(builder.Configuration.GetSection(DemoUserSettings.SectionName))
                .Validate(settings => !settings.SeedEnabled || (!string.IsNullOrWhiteSpace(settings.Email) && !string.IsNullOrWhiteSpace(settings.Password)),
                    "Demo user email and password are required when seeding is enabled.")
                .ValidateOnStart();

builder.Services.AddSingleton<IMongoClient>(ServiceProvider =>
{
    var settings = ServiceProvider
                    .GetRequiredService<IOptions<MongoSettings>>()
                    .Value;

    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(ServiceProvider =>
{
    var settings = ServiceProvider
                     .GetRequiredService<IOptions<MongoSettings>>()
                     .Value;

    var client = ServiceProvider.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

builder.Services.AddScoped<IPasswordHasher<UserDocument>, PasswordHasher<UserDocument>>();
builder.Services.AddScoped<DevUserSeeder>();
builder.Services.AddScoped<ICasesService, CasesService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    var seedSettings = app.Services.GetRequiredService<IOptions<DemoUserSettings>>().Value;
    if (seedSettings.SeedEnabled)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DevUserSeeder>();
        await seeder.SeedAsync(app.Lifetime.ApplicationStopped);
    }
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapHealthChecks("/health");

app.MapControllers();

await app.RunAsync();