using System.Text.Json.Serialization;

namespace Instructor.NET.Models;

public class UserProfile : ResponseModel
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("preferred_language")]
    public string PreferredLanguage { get; set; } = string.Empty;
}