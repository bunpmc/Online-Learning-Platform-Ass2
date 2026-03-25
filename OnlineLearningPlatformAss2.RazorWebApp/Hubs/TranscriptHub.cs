using Microsoft.AspNetCore.SignalR;
using OnlineLearningPlatformAss2.Service.Services.Interfaces;
using System.Security.Claims;

namespace OnlineLearningPlatformAss2.RazorWebApp.Hubs;

public class TranscriptHub(ITranscriptService transcriptService, ICourseService courseService, IChatbotService chatbotService) : Hub
{
    private readonly ITranscriptService _transcriptService = transcriptService;
    private readonly ICourseService _courseService = courseService;
    private readonly IChatbotService _chatbotService = chatbotService;

    public async Task GenerateSummary(Guid courseId, Guid lessonId)
    {
        var userIdString = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            await Clients.Caller.SendAsync("ReceiveError", "User not authorized.");
            return;
        }

        try
        {
            var enrollmentId = await _courseService.GetEnrollmentIdAsync(userId, courseId);
            if (!enrollmentId.HasValue)
            {
                await Clients.Caller.SendAsync("ReceiveError", "You are not enrolled in this course.");
                return;
            }

            // 1. Get lesson data with existing AI content
            var lesson = (await _courseService.GetCourseLearnAsync(enrollmentId.Value))?
                         .Modules.SelectMany(m => m.Lessons).FirstOrDefault(l => l.Id == lessonId);
            
            if (lesson == null || string.IsNullOrEmpty(lesson.VideoUrl))
            {
                 await Clients.Caller.SendAsync("ReceiveError", "Video not available.");
                 return;
            }

            // Check if we already have summary. If yes, return it immediately.
            if (!string.IsNullOrEmpty(lesson.AiSummary)) 
            {
                await Clients.Caller.SendAsync("ReceiveSummary", lesson.AiSummary);
                return;
            }

            // 2. Get or Generate Transcript
            string fullTranscript = lesson.Transcript ?? string.Empty;
            
            if (string.IsNullOrEmpty(fullTranscript))
            {
                fullTranscript = await _transcriptService.GenerateTranscriptFromVideoAsync(lesson.VideoUrl, false); // false = no summary
            }
            
            if (string.IsNullOrEmpty(fullTranscript)) 
            {
                 await Clients.Caller.SendAsync("ReceiveError", "Failed to generate transcript.");
                 return;
            }

            // 3. Generate Summary via Groq
            var groqSummary = await _chatbotService.SummarizeTranscriptAsync(fullTranscript);

            // 4. Save to database
            await _courseService.SaveLessonAiDataAsync(enrollmentId.Value, lessonId, fullTranscript, groqSummary);

            // 5. Return
            await Clients.Caller.SendAsync("ReceiveSummary", groqSummary);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ReceiveError", $"Error: {ex.Message}");
        }
    }
}
