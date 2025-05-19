using System.Text.Json.Serialization;

namespace Instructor.NET.Models;

/// <summary>
/// Base class for all response models
/// </summary>
public abstract class ResponseModel
{
    [JsonIgnore]
    public string? RawResponse { get; set; }
}