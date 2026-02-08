using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OnlineLearningPlatformAss2.Service.Services.Interfaces;

namespace OnlineLearningPlatformAss2.RazorWebApp.Pages.Payment;

public class CallbackModel : PageModel
{
    private readonly IVnPayService _vnPayService;
    private readonly IOrderService _orderService;
    private readonly ILogger<CallbackModel> _logger;

    public CallbackModel(IVnPayService vnPayService, IOrderService orderService, ILogger<CallbackModel> logger)
    {
        _vnPayService = vnPayService;
        _orderService = orderService;
        _logger = logger;
    }

    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string PaymentDate { get; set; } = string.Empty;
    
    // For retry
    public Guid OriginalId { get; set; }
    public string OriginalType { get; set; } = "Course";

    public async Task<IActionResult> OnGetAsync()
    {
        _logger.LogInformation("--- Callback.OnGetAsync ---");
        foreach (var key in Request.Query.Keys)
        {
            _logger.LogInformation("Query: {Key} = {Value}", key, Request.Query[key]);
        }

        var response = _vnPayService.PaymentExecute(Request.Query);
        _logger.LogInformation("PaymentExecute Result: Success={Success}, Code={Code}, OrderId={OrderId}", 
            response?.Success, response?.VnPayResponseCode, response?.OrderId);

        if (response == null || !response.Success)
        {
            Success = false;
            Message = $"Payment validation failed. Code: {response?.VnPayResponseCode}";
            _logger.LogWarning("Payment validation failed: {Message}", Message);
            return Page();
        }

        // Validate and complete order
        if (Guid.TryParse(response.OrderId, out var orderGuid))
        {
            var result = await _orderService.ProcessPaymentAsync(orderGuid, "VNPay");
            _logger.LogInformation("ProcessPaymentAsync Result: {Result}", result);
            
            if (result)
            {
                Success = true;
                OrderId = response.OrderId;
                TransactionId = response.TransactionId;
                
                // Fetch full order details
                var orderDetails = await _orderService.GetOrderDetailsAsync(orderGuid);
                if (orderDetails != null)
                {
                    Amount = orderDetails.TotalAmount;
                    ItemName = orderDetails.Items.FirstOrDefault()?.Title ?? "Unknown Course/Path";
                    PaymentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    
                    // Set original ID for "Try Again" fallback/navigation (though typically not needed for success)
                    // If we needed to navigate back to course, we'd use orderDetails.Items.FirstOrDefault()?.Id
                }
            }
            else
            {
                Success = false;
                Message = "Order processing failed after payment.";
                _logger.LogError("Order processing failed for OrderId: {OrderId}", orderGuid);
            }
        }
        else
        {
            Success = false;
            Message = "Invalid Order ID returned from payment gateway.";
            _logger.LogError("Invalid Order ID format: {OrderId}", response.OrderId);
        }

        return Page();
    }
}
