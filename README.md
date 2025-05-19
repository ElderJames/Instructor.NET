# Instructor.NET

A .NET port of the Python [Instructor](https://github.com/567-labs/instructor) library for structured outputs from large language models.

## Features

- Strongly typed responses from LLMs
- Built-in support for OpenAI's GPT models
- Easy integration with Azure OpenAI
- JSON schema-based response validation
- Customizable output formatting

## Installation

```bash
dotnet add package Instructor.NET
```

## Quick Start

1. First, create a model that inherits from `ResponseModel`:

```csharp
public class UserProfile : ResponseModel
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("preferred_language")]
    public string PreferredLanguage { get; set; } = string.Empty;
}
```

2. Initialize the client with your API key:

```csharp
var client = new InstructorClient("your-api-key-here");
```

3. Create structured outputs:

```csharp
var userProfile = await client.CreateStructuredOutput<UserProfile>(
    "Create a profile for a software developer named Alice who is 28 years old, " +
    "uses alice@example.com, and prefers JavaScript"
);

Console.WriteLine($"Name: {userProfile.Name}");
Console.WriteLine($"Age: {userProfile.Age}");
Console.WriteLine($"Email: {userProfile.Email}");
Console.WriteLine($"Preferred Language: {userProfile.PreferredLanguage}");
```

## Advanced Usage

### Custom Temperature and Token Limits

```csharp
var profile = await client.CreateStructuredOutput<UserProfile>(
    "Create a profile for an experienced developer",
    temperature: 0.9f,
    maxTokens: 1000
);
```

### Using Azure OpenAI

```csharp
var client = new InstructorClient(
    "your-api-key-here",
    "https://your-resource-name.openai.azure.com/"
);
```

### Accessing Raw Response

```csharp
var profile = await client.CreateStructuredOutput<UserProfile>(prompt);
Console.WriteLine($"Raw JSON response: {profile.RawResponse}");
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
