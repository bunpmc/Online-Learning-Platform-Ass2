namespace OnlineLearningPlatformAss2.Service.DTOs.Course;

public class CertificateViewModel
{
    public Guid CertificateId { get; set; }
    public string Username { get; set; } = "";
    public string CourseTitle { get; set; } = "";
    public string InstructorName { get; set; } = "";
    public DateTime IssuedDate { get; set; }
    public string VerificationCode { get; set; } = "";
}
