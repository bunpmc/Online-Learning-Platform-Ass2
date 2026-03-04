using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineLearningPlatformAss2.Service.Services.Interfaces;
using OnlineLearningPlatformAss2.Service.DTOs.Order;
using System.Security.Claims;
using OnlineLearningPlatformAss2.Service.Services;
using Microsoft.Extensions.Logging;

namespace OnlineLearningPlatformAss2.RazorWebApp.Pages.Order;

[Authorize]
public class CheckoutModel : PageModel
{
    private readonly IOrderService _orderService;
    private readonly ICourseService _courseService;
    private readonly ILearningPathService _learningPathService;
    private readonly IVnPayService _vnPayService;
    private readonly IUserService _userService;
    private readonly ILogger<CheckoutModel> _logger;

    public CheckoutModel(
        IOrderService orderService, 
        ICourseService courseService, 
        ILearningPathService learningPathService,
        IVnPayService vnPayService,
        IUserService userService,
        ILogger<CheckoutModel> logger)
    {
        _orderService = orderService;
        _courseService = courseService;
        _learningPathService = learningPathService;
        _vnPayService = vnPayService;
        _userService = userService;
        _logger = logger;
    }

    [BindProperty]
    public Guid ItemId { get; set; }

    [BindProperty]
    public string ItemType { get; set; } = "Course"; // Course or LearningPath

    [BindProperty]
    public string PaymentMethod { get; set; } = "VNPay";

    public string ItemTitle { get; set; } = "";
    public decimal ItemPrice { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, string type = "Course")
    {
        ItemId = id;
        ItemType = type;

        if (type == "Course")
        {
            var course = await _courseService.GetCourseDetailsAsync(id);
            if (course == null) return NotFound();
            
            // Check if already enrolled
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var userId))
            {
                var enrolled = await _courseService.GetEnrolledCoursesAsync(userId);
                if (enrolled.Any(c => c.Id == id))
                {
                    TempData["InfoMessage"] = "You are already enrolled in this course.";
                    return RedirectToPage("/Course/Details", new { id = id });
                }
            }

            ItemTitle = course.Title;
            ItemPrice = course.Price;
        }
        else
        {
            var path = await _learningPathService.GetLearningPathDetailsAsync(id);
            if (path == null) return NotFound();
            ItemTitle = path.Title;
            ItemPrice = path.Price;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId))
        {
            return RedirectToPage("/User/Login");
        }

        // Verify user exists in database to prevent FK constraint violation
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User ID {UserId} from claims not found in database. Redirecting to login.", userId);
            // Ideally sign out here, but redirecting to login is a safe fallback
            return RedirectToPage("/User/Login"); 
        }

        try
        {
            OrderViewModel order;
            // Create pending order
            if (ItemType == "Course")
            {
                order = await _orderService.CreateCourseOrderAsync(userId, ItemId);
            }
            else
            {
                order = await _orderService.CreateLearningPathOrderAsync(userId, ItemId);
            }

            // DEBUG LOGS
            _logger.LogInformation("Order created: {OrderId}", order.Id);
            _logger.LogInformation("Payment Method: {PaymentMethod}", PaymentMethod);

            if (PaymentMethod == "VNPay")
            {
                var vnPayModel = new Service.DTOs.VnPay.VnPayRequestModel
                {
                    Amount = (double)order.TotalAmount,
                    CreatedDate = DateTime.Now,
                    OrderDescription = $"{ItemType} Payment",
                    FullName = User.Identity?.Name ?? "Customer",
                    OrderId = order.Id
                };

                var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, vnPayModel);
                
                // DEBUG LOGS
                _logger.LogInformation("Payment URL: {PaymentUrl}", paymentUrl);
                
                return Redirect(paymentUrl);
            }

            ModelState.AddModelError("", "Invalid payment method selected.");
            return await OnGetAsync(ItemId, ItemType);
        }
        catch (Exception ex)
        {
            // ERROR LOGS
            _logger.LogError(ex, "Checkout Error: {Message}", ex.Message);
            
            ModelState.AddModelError("", ex.Message);
            return await OnGetAsync(ItemId, ItemType);
        }
    }
}
