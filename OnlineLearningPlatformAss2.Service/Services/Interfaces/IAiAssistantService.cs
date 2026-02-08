namespace OnlineLearningPlatformAss2.Service.Services.Interfaces;

public interface IAiAssistantService
{
    Task<string> GetChatResponseAsync(string userMessage, string systemContext);
}
