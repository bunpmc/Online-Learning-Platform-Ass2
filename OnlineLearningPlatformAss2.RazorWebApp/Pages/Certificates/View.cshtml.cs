using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineLearningPlatformAss2.Service.Services.Interfaces;

namespace OnlineLearningPlatformAss2.RazorWebApp.Pages.Certificates;

public class ViewModel : PageModel
{
    private readonly ICourseService _courseService;

    public ViewModel(ICourseService courseService)
    {
        _courseService = courseService;
    }

    public string Username { get; set; } = "";
    public string CourseTitle { get; set; } = "";
    public string InstructorName { get; set; } = "";
    public DateTime IssuedDate { get; set; }
    public Guid CertificateId { get; set; }
    public string VerificationCode { get; set; } = "";

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var cert = await _courseService.GetCertificateAsync(id);

        if (cert == null) return NotFound();

        Username = cert.Username;
        CourseTitle = cert.CourseTitle;
        InstructorName = cert.InstructorName;
        IssuedDate = cert.IssuedDate;
        CertificateId = cert.CertificateId;
        VerificationCode = cert.VerificationCode;

        return Page();
    }
}
