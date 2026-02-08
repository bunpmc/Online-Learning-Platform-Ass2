using System.Net.Http.Headers;
using System.Text.Json;
using OnlineLearningPlatformAss2.Service.Services.Interfaces;

namespace OnlineLearningPlatformAss2.Service.Services;

public class TranscriptService(HttpClient httpClient) : ITranscriptService
{
    private readonly HttpClient _http = httpClient;

    public async Task<string> GenerateTranscriptFromVideoAsync(string videoUrl)
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            // Download file to temp
            await using (var stream = await _http.GetStreamAsync(videoUrl))
            await using (var fs = File.Create(tempFile))
            {
                await stream.CopyToAsync(fs);
            }

            using var form = new MultipartFormDataContent();

            await using var fs2 = File.OpenRead(tempFile);
            var fileContent = new StreamContent(fs2);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");

            form.Add(fileContent, "file", Path.GetFileName(tempFile));

            var response = await _http.PostAsync(
                "http://100.86.222.32:8000/transcript",
                form
            );

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            
            // Use System.Text.Json instead of Newtonsoft.Json
            using var doc = JsonDocument.Parse(json);
            
            if (doc.RootElement.TryGetProperty("segments", out var segmentsElement) && segmentsElement.ValueKind == JsonValueKind.Array)
            {
                var sb = new System.Text.StringBuilder();
                foreach (var segment in segmentsElement.EnumerateArray())
                {
                    if (segment.TryGetProperty("start", out var startProc) && segment.TryGetProperty("text", out var textProc))
                    {
                        var start = startProc.GetDouble();
                        var text = textProc.GetString() ?? "";
                        sb.AppendLine($"[{TimeSpan.FromSeconds(start):hh\\:mm\\:ss}] {text.Trim()}");
                    }
                }
                var result = sb.ToString();
                if (!string.IsNullOrWhiteSpace(result))
                    return result;
            }

            if (doc.RootElement.TryGetProperty("text", out var textElement))
            {
                return textElement.GetString() ?? string.Empty;
            }
            
            return string.Empty;
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
