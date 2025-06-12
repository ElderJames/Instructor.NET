using Instructor.NET.OpenAI;
using Xunit;

namespace Instructor.NET.Tests;

public class InstructorClientTests
{
    private readonly InstructorClient _client;

    public InstructorClientTests()
    {
        // Replace with your API key for testing
        _client = new InstructorClient("sk-7ff80b32746640a08f6f3c8e5c5ffcd8",endpoint:"https://api.deepseek.com/v1");
    }

    [Fact]
    public async Task CreateStructuredOutput_ShouldReturnValidUserProfile()
    {
        // Arrange
        var prompt = "Create a profile for a software developer named John who is 30 years old, uses john@example.com, and prefers Python";

        // Act
        var result = await _client.CreateStructuredOutput<UserProfile>(prompt,"deepseek-chat");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.Name);
        Assert.Equal(30, result.Age);
        Assert.Equal("john@example.com", result.Email);
        Assert.Equal("Python", result.PreferredLanguage);
        Assert.NotNull(result.RawResponse);
    }
}