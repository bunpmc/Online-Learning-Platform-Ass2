using System;
using System.Collections.Generic;

namespace OnlineLearningPlatformAss2.Data.Entities;

public partial class Certificate
{
    public Guid CertificateId { get; set; }

    public Guid EnrollmentId { get; set; }

    public string SerialNumber { get; set; } = null!;

    public DateTime IssueDate { get; set; }

    public string? PdfUrl { get; set; }

    public virtual Enrollment Enrollment { get; set; } = null!;
}
