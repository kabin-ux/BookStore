using System.Net.Mail;
using System.Net;

namespace BookStore.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        public async Task SendEmailAsync(string code, string toEmail)
        {
            var subject = "Your Verification Code";
            var body = $"Your 6-digit verification code is: {code}";
            using var smtpClient = new SmtpClient(_config["Smtp:Host"])
            {
                Port = int.Parse(_config["Smtp:Port"]),
                Credentials = new NetworkCredential(_config["Smtp:Username"], _config["Smtp:Password"]),
                EnableSsl = true,
            };

            var message = new MailMessage
            {
                From = new MailAddress(_config["Smtp:From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };
            message.To.Add(toEmail);

            await smtpClient.SendMailAsync(message);
        }
    }
}
