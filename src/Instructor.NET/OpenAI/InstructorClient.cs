using Instructor.NET.Models;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

namespace Instructor.NET.OpenAI;

public class InstructorClient
{
    private readonly OpenAIClient _client;
    private readonly static JsonSerializerOptions _jsonOptions = new(JsonSerializerOptions.Default)
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private readonly static JsonSchemaExporterOptions _exporterOptions = new()
    {
        TransformSchemaNode = (context, schema) =>
        {
            // Determine if a type or property and extract the relevant attribute provider.
            ICustomAttributeProvider? attributeProvider = context.PropertyInfo is not null
                ? context.PropertyInfo.AttributeProvider
                : context.TypeInfo.Type;

            // Look up any description attributes.
            DescriptionAttribute? descriptionAttr = attributeProvider?
                .GetCustomAttributes(inherit: true)
                .Select(attr => attr as DescriptionAttribute)
                .FirstOrDefault(attr => attr is not null);

            // Apply description attribute to the generated schema.
            if (descriptionAttr != null)
            {
                if (schema is not JsonObject jObj)
                {
                    // Handle the case where the schema is a Boolean.
                    JsonValueKind valueKind = schema.GetValueKind();
                    Debug.Assert(valueKind is JsonValueKind.True or JsonValueKind.False);
                    schema = jObj = new JsonObject();
                    if (valueKind is JsonValueKind.False)
                    {
                        jObj.Add("not", true);
                    }
                }

                jObj.Insert(0, "description", descriptionAttr.Description);
            }

            return schema;
        }
    };

    public InstructorClient(string apiKey, string? endpoint = null)
    {
        _client = string.IsNullOrEmpty(endpoint)
            ? new OpenAIClient(apiKey)
            : new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions
            {
                Endpoint = new Uri(endpoint)
            });
    }

    public async Task<T> CreateStructuredOutput<T>(
        string prompt,
        string model = "gpt-4",
        float temperature = 0.7f,
        int maxTokens = 800) where T : ResponseModel
    {
        // Get the schema for type T
        var schema = _jsonOptions.GetJsonSchemaAsNode(typeof(T), _exporterOptions);

        var systemPrompt = $@"
You are a helpful assistant that generates structured data.
You must respond with valid JSON that matches the following schema:
{schema}

Format your entire response as JSON that can be parsed by a JSON parser.
";

        var chatCompletionsOptions = new ChatCompletionOptions
        {
            Temperature = temperature,
            MaxOutputTokenCount = maxTokens,
            //NucleusSamplingFactor = 0.95f,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };

        ChatMessage[] messages = [
            ChatMessage.CreateSystemMessage(systemPrompt),
            ChatMessage.CreateUserMessage(prompt)
        ];

        var chatClient = _client.GetChatClient(model);
        var response = await chatClient.CompleteChatAsync(messages, chatCompletionsOptions);
        var jsonResponse = response.Value.Content[0].Text ?? string.Empty;

        var result = JsonExtractor.ExtractObject<T>(jsonResponse);
        if (result != null)
        {
            result.RawResponse = jsonResponse;
        }
        return result ?? throw new InvalidOperationException("Failed to deserialize response");
    }
}