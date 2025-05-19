using Instructor.NET.Models;
using Instructor.NET.OpenAI;

namespace Instructor.NET.Examples;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Initialize the client with your API key
        var client = new InstructorClient("your-api-key-here");

        // Example 1: Create a structured output
        Console.WriteLine("Creating a structured user profile...");
        var userProfile = await client.CreateStructuredOutput<UserProfile>(
            "Create a profile for a software developer named Alice who is 28 years old, " +
            "uses alice@example.com, and prefers JavaScript"
        );

        Console.WriteLine($"Created profile for {userProfile.Name}:");
        Console.WriteLine($"Age: {userProfile.Age}");
        Console.WriteLine($"Email: {userProfile.Email}");
        Console.WriteLine($"Preferred Language: {userProfile.PreferredLanguage}");
        Console.WriteLine($"Raw Response: {userProfile.RawResponse}");

        // Example 2: Create a completion with different temperature
        Console.WriteLine("\nCreating another profile with different settings...");
        var anotherProfile = await client.CreateStructuredOutput<UserProfile>(
            "Create a profile for an experienced developer",
            temperature: 0.9f,
            maxTokens: 1000
        );

        Console.WriteLine($"Created profile for {anotherProfile.Name}:");
        Console.WriteLine($"Age: {anotherProfile.Age}");
        Console.WriteLine($"Email: {anotherProfile.Email}");
        Console.WriteLine($"Preferred Language: {anotherProfile.PreferredLanguage}");
    }
}