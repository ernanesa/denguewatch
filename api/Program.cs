using System.Text;
using api.Configuration;
using api.Data.Documents;
using api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
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

builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
    .Validate(token => !string.IsNullOrWhiteSpace(token.Issuer), "Jwt: Issuer ir required.")
    .Validate(token => !string.IsNullOrWhiteSpace(token.Audience), "Jwt: Audience ir required.")
    .Validate(token => !string.IsNullOrWhiteSpace(token.Key), "Jwt: Key ir required.")
    .Validate(token => token.Key.Length >= 32, "Jwt: Key must be at least 32 characters.")
    .Validate(token => token.ExpiryMinutes > 0, "Jwt:ExpiryMinutes must be positive.")
    .ValidateOnStart();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwt = builder.Configuration
                                     .GetSection(JwtSettings.SectionName)
                                     .Get<JwtSettings>()!;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwt.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwt.Key)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            In = ParameterLocation.Header,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Informe o JWT retornado por POST /api/Auth/login."
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes[
            JwtBearerDefaults.AuthenticationScheme] = scheme;

        return Task.CompletedTask;
    });
});

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
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers();
// builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var seedSettings = app.Services.GetRequiredService<IOptions<DemoUserSettings>>().Value;
    if (seedSettings.SeedEnabled)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DevUserSeeder>();
        await seeder.SeedAsync(app.Lifetime.ApplicationStopping);
    }
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

await app.RunAsync();