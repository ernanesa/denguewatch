namespace api.DTOs.Cases;

public sealed record CaseResponse(
    string Id,
    DateTime DateOfBirth,
    string Sex,
    double Latitude,
    double Longitude
);
