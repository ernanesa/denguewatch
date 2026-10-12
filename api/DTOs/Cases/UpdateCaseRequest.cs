using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Cases;

public class UpdateCaseRequest
{
    [Required]
    public DateTime? DateOfBirth { get; init; }

    [Required]
    [StringLength(20)]
    public string? Sex { get; init; }

    [Required]
    [Range(-90, 90)]
    public double? Latitude { get; init; }

    [Required]
    [Range(-180, 180)]
    public double? Longitude { get; init; }
}
