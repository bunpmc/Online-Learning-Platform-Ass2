using OnlineLearningPlatformAss2.Data.Entities;
using OnlineLearningPlatformAss2.Data.Repositories.Interfaces;
using OnlineLearningPlatformAss2.Service.DTOs.Quiz;
using OnlineLearningPlatformAss2.Service.Services.Interfaces;

namespace OnlineLearningPlatformAss2.Service.Services;

public class QuizService(IQuizRepository quizRepository) : IQuizService
{
    public async Task<QuizViewModel?> GetQuizForLessonAsync(Guid lessonId)
    {
        var quiz = await quizRepository.GetQuizByLessonAsync(lessonId);
        if (quiz == null) return null;

        return new QuizViewModel
        {
            Id = quiz.QuizId,
            Title = quiz.Title,
            Questions = quiz.Questions.Select(q => new QuestionViewModel
            {
                Id = q.QuestionId,
                Text = q.Content,
                Options = q.Options.Select(o => new OptionViewModel
                {
                    Id = o.OptionId,
                    Text = o.Text
                }).ToList()
            }).ToList()
        };
    }

    public async Task<QuizResultDto> SubmitAttemptAsync(Guid userId, QuizSubmissionDto submission)
    {
        var quiz = await quizRepository.GetQuizWithDetailsAsync(submission.QuizId);
        if (quiz == null) throw new ArgumentException("Quiz not found");

        int correctCount = 0;
        foreach (var answer in submission.Answers)
        {
            var question = quiz.Questions.FirstOrDefault(q => q.QuestionId == answer.QuestionId);
            if (question != null)
            {
                var originalOption = question.Options.FirstOrDefault(o => o.OptionId == answer.SelectedOptionId);
                if (originalOption != null && originalOption.IsCorrect)
                {
                    correctCount++;
                }
            }
        }

        int score = (int)Math.Round((double)correctCount / quiz.Questions.Count * 100);
        bool passed = score >= quiz.PassingScore;

        var attempt = new QuizAttempt
        {
            AttemptId = Guid.NewGuid(),
            UserId = userId,
            QuizId = quiz.QuizId,
            Score = score,
            Passed = passed,
            AttemptedAt = DateTime.UtcNow
        };

        await quizRepository.AddAttemptAsync(attempt);
        await quizRepository.SaveChangesAsync();

        return new QuizResultDto
        {
            Score = score,
            TotalQuestions = quiz.Questions.Count,
            Passed = passed
        };
    }

    public async Task<bool> HasPassedQuizAsync(Guid userId, Guid quizId)
    {
        return await quizRepository.HasPassedQuizAsync(userId, quizId);
    }
}
