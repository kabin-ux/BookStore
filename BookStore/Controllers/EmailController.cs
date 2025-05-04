using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")] // Adjust role if needed
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendCode()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized("Email not found in token.");

            var code = new Random().Next(100000, 999999).ToString();
            var subject = "Your Verification Code";
            var body = $"Your 6-digit verification code is: {code}";

            await _emailService.SendEmailAsync(email, subject, body);

            return Ok(new { message = "Code sent to your email." /*, code */ }); // Optionally include code for dev
        }
    }
}
