using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using OnlineLearningPlatformAss2.Service.Services.Interfaces;

namespace OnlineLearningPlatformAss2.Service.Services;

public class GroqApiService : IAiAssistantService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string ApiUrl = "https://api.groq.com/openai/v1/chat/completions";

    public GroqApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["GroqAPIKey:Key"] ?? throw new InvalidOperationException("Groq API Key (GroqAPIKey:Key) is missing in appsettings.");
    }

    public async Task<string> GetChatResponseAsync(string userMessage, string systemContext)
    {
        var requestBody = new
        {
            model = "llama3-70b-8192", // Using a capable model available on Groq
            messages = new[]
            {
                new { role = "system", content = systemContext },
                new { role = "user", content = userMessage }
            },
            temperature = 0.7
        };

        var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        request.Content = JsonContent.Create(requestBody);

        try 
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<GroqResponse>(responseString);

            return responseData?.Choices?.FirstOrDefault()?.Message?.Content ?? "Sorry, I couldn't generate a response.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Groq API Error: {ex.Message}");
            return "I'm having trouble connecting to the AI service right now. Please try again later.";
        }
    }

    // Helper classes for deserialization
    private class GroqResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice>? Choices { get; set; }
    }

    private class Choice
    {
        [JsonPropertyName("message")]
        public Message? Message { get; set; }
    }

    private class Message
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}
