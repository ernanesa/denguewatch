using api.Data.Documents;

namespace api.Services;

public interface IUsersService
{
    Task<UserDocument?> FindByEmailAsync(string email, CancellationToken cancellationToken);
}
