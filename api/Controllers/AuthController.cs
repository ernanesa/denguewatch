using api.Data.Documents;
using api.DTOs.Auth;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IUsersService users, IPasswordHasher<UserDocument> passwordHasher, ITokenService tokenService) : ControllerBase
    {
        private readonly IUsersService _users = users;
        private readonly IPasswordHasher<UserDocument> _passwordHasher = passwordHasher;
        private readonly ITokenService _tokenService = tokenService;

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _users.FindByEmailAsync(request.Email, cancellationToken);

            if (user is null) return Unauthorized();

            var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verification == PasswordVerificationResult.Failed) return Unauthorized();

            var token = _tokenService.Create(user);

            return Ok(token);
        }
    }
}