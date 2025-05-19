using Instructor.NET.Models;
using Instructor.NET.OpenAI;
using Xunit;

namespace Instructor.NET.Tests;

public class InstructorClientTests
{
    private readonly InstructorClient _client;

    public InstructorClientTests()
    {
        // Replace with your API key for testing
        _client = new InstructorClient("your-api-key-here");
    }

    [Fact]
    public async Task CreateStructuredOutput_ShouldReturnValidUserProfile()
    {
        // Arrange
        var prompt = "Create a profile for a software developer named John who is 30 years old, uses john@example.com, and prefers Python";

        // Act
        var result = await _client.CreateStructuredOutput<UserProfile>(prompt);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.Name);
        Assert.Equal(30, result.Age);
        Assert.Equal("john@example.com", result.Email);
        Assert.Equal("Python", result.PreferredLanguage);
        Assert.NotNull(result.RawResponse);
    }

    [Fact]
    public async Task CreateCompletion_ShouldReturnValidUserProfile()
    {
        // Arrange
        var prompt = "Create a JSON profile for a software developer with the following properties: name, age, email, and preferred_language";

        // Act
        var result = await _client.CreateCompletion<UserProfile>(prompt);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Name);
        Assert.True(result.Age > 0);
        Assert.NotEmpty(result.Email);
        Assert.NotEmpty(result.PreferredLanguage);
        Assert.NotNull(result.RawResponse);
    }
}