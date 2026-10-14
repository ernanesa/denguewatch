using api.Data.Documents;
using MongoDB.Driver;

namespace api.Services;

public class UsersService(IMongoDatabase database) : IUsersService
{
    private readonly IMongoCollection<UserDocument> _users = database.GetCollection<UserDocument>("users");
    public async Task<UserDocument?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToUpperInvariant();
        return await _users
                        .Find(user => user.NormalizedEmail == normalizedEmail)
                        .FirstOrDefaultAsync(cancellationToken);
    }
}
