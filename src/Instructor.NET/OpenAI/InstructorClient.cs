using System.Text.Json;
using Azure.AI.OpenAI;
using Instructor.NET.Models;

namespace Instructor.NET.OpenAI;

public class InstructorClient
{
    private readonly OpenAIClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public InstructorClient(string apiKey, string? endpoint = null)
    {
        _client = string.IsNullOrEmpty(endpoint)
            ? new OpenAIClient(apiKey)
            : new OpenAIClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<T> CreateCompletion<T>(string prompt, string model = "gpt-4") where T : ResponseModel
    {
        var chatCompletionsOptions = new ChatCompletionsOptions
        {
            Messages =
            {
                new ChatMessage(ChatRole.System, "You are a helpful assistant that responds in JSON format."),
                new ChatMessage(ChatRole.User, prompt)
            },
            Temperature = 0.7f,
            MaxTokens = 800,
            NucleusSamplingFactor = 0.95f,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };

        var response = await _client.GetChatCompletionsAsync(model, chatCompletionsOptions);
        var jsonResponse = response.Value.Choices[0].Message.Content;

        var result = JsonSerializer.Deserialize<T>(jsonResponse, _jsonOptions);
        if (result != null)
        {
            result.RawResponse = jsonResponse;
        }
        return result ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public async Task<T> CreateStructuredOutput<T>(
        string prompt,
        string model = "gpt-4",
        float temperature = 0.7f,
        int maxTokens = 800) where T : ResponseModel
    {
        // Get the schema for type T
        var schema = JsonSerializer.SerializeToDocument(
            new { type = typeof(T).Name, properties = typeof(T).GetProperties() },
            _jsonOptions
        ).RootElement.ToString();

        var systemPrompt = $@"
You are a helpful assistant that generates structured data.
You must respond with valid JSON that matches the following schema:
{schema}

Format your entire response as JSON that can be parsed by a JSON parser.
";

        var chatCompletionsOptions = new ChatCompletionsOptions
        {
            Messages =
            {
                new ChatMessage(ChatRole.System, systemPrompt),
                new ChatMessage(ChatRole.User, prompt)
            },
            Temperature = temperature,
            MaxTokens = maxTokens,
            NucleusSamplingFactor = 0.95f,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };

        var response = await _client.GetChatCompletionsAsync(model, chatCompletionsOptions);
        var jsonResponse = response.Value.Choices[0].Message.Content;

        var result = JsonSerializer.Deserialize<T>(jsonResponse, _jsonOptions);
        if (result != null)
        {
            result.RawResponse = jsonResponse;
        }
        return result ?? throw new InvalidOperationException("Failed to deserialize response");
    }
}