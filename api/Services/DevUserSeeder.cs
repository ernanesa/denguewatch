using api.Configuration;
using api.Data.Documents;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace api.Services;

public sealed class DevUserSeeder(
    IMongoDatabase database, IOptions<DemoUserSettings> options, IPasswordHasher<UserDocument> passwordHasher,
    IHostEnvironment hostEnvironment, ILogger<DevUserSeeder> logger)
{
    private readonly IMongoCollection<UserDocument> _users = database.GetCollection<UserDocument>("users");
    private readonly DemoUserSettings _settings = options.Value;
    private readonly IPasswordHasher<UserDocument> _passwordHasher = passwordHasher;
    private readonly IHostEnvironment _environment = hostEnvironment;
    private readonly ILogger<DevUserSeeder> _logger = logger;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_environment.IsDevelopment())
        {
            _logger.LogInformation("Demo user seeding skipped outside Development.");
            return;
        }

        if (!_settings.SeedEnabled)
        {
            _logger.LogInformation("Demo user seeding is disablead.");
            return;
        }

        var email = _settings.Email.Trim();
        var normalizedEmail = email.ToUpperInvariant();

        var emailIndex = new CreateIndexModel<UserDocument>(
            Builders<UserDocument>.IndexKeys
            .Ascending(user => user.NormalizedEmail),
            new CreateIndexOptions
            {
                Unique = true,
                Name = "ux_users_normalized_email"
            }
        );
        await _users.Indexes.CreateOneAsync(emailIndex, cancellationToken: cancellationToken);

        var existingUser = await _users.Find(user => user.NormalizedEmail == normalizedEmail).FirstOrDefaultAsync(cancellationToken);
        if (existingUser is not null)
        {
            _logger.LogInformation("Development demo user already exists. Skipping.");
            return;
        }

        var user = new UserDocument
        {
            Email = email,
            NormalizedEmail = normalizedEmail
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, _settings.Password);
        await _users.InsertOneAsync(user, new InsertOneOptions(), cancellationToken);

        _logger.LogInformation("Development demo user was seeded.");
    }
}
