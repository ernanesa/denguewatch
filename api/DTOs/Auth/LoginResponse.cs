namespace api.DTOs.Auth;

public record LoginResponse(string AccessToken, DateTime ExpiresAtUtc);
