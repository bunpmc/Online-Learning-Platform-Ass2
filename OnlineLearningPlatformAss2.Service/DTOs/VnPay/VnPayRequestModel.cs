namespace OnlineLearningPlatformAss2.Service.DTOs.VnPay;

public class VnPayRequestModel
{
    public Guid OrderId { get; set; }
    public string FullName { get; set; }
    public string OrderDescription { get; set; }
    public double Amount { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Locale { get; set; }
    public string? BankCode { get; set; }
    public DateTime? ExpireDate { get; set; }
}