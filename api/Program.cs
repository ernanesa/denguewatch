using api.Configuration;
using api.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<MongoSettings>()
                .Bind(builder.Configuration.GetSection(MongoSettings.SectionName))
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.ConnectionString), "MongoSettings:ConnectionString must be provided.")
                .Validate(settings => !string.IsNullOrWhiteSpace(settings.DatabaseName), "MongoSettings:DatabaseName must be provided.")
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

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();


// DI
builder.Services.AddScoped<ICasesService, CasesService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();