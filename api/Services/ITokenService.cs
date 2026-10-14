using api.Data.Documents;
using api.DTOs.Auth;

namespace api.Services;

public interface ITokenService
{
    LoginResponse Create(UserDocument user);
}
