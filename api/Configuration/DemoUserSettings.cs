namespace api.Configuration;

public class DemoUserSettings
{
    public const string SectionName = "DemoUser";

    public bool SeedEnabled { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
