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

        public async Task SendOrderConfirmationAsync(string code, string toEmail, long orderId, decimal billAmount, decimal finalAmount)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code), "Verification code is null or empty");

            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentNullException(nameof(toEmail), "Recipient email is null or empty");

            var host = _config["Smtp:Host"] ?? throw new InvalidOperationException("SMTP Host is not configured");
            var portString = _config["Smtp:Port"] ?? throw new InvalidOperationException("SMTP Port is not configured");
            var from = _config["Smtp:From"] ?? throw new InvalidOperationException("SMTP From address is not configured");
            var username = _config["Smtp:Username"] ?? throw new InvalidOperationException("SMTP Username is not configured");
            var password = _config["Smtp:Password"] ?? throw new InvalidOperationException("SMTP Password is not configured");

            if (!int.TryParse(portString, out int port))
                throw new InvalidOperationException("SMTP Port is not a valid integer");

            using var smtpClient = new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
            };

            var message = new MailMessage
            {
                From = new MailAddress(from),
                Subject = "Your Order Confirmation",
                Body = $@"
                    Thank you for your order!
                    
                    Order ID: {orderId}
                    Claim Code: {code}
                    Bill Amount: Rs.{billAmount:F2}
                    Final Amount After Discount: Rs.{finalAmount:F2}
                    
                    Please keep this information for your records.
                ",
                IsBodyHtml = false,
            };
            message.To.Add(toEmail);

            await smtpClient.SendMailAsync(message);
        }
        public async Task SendOrderCancellationToStaffAsync(string staffEmail, string userName, int orderId)
        {

            var host = _config["Smtp:Host"] ?? throw new InvalidOperationException("SMTP Host is not configured");
            var portString = _config["Smtp:Port"] ?? throw new InvalidOperationException("SMTP Port is not configured");
            var from = _config["Smtp:From"] ?? throw new InvalidOperationException("SMTP From address is not configured");
            var username = _config["Smtp:Username"] ?? throw new InvalidOperationException("SMTP Username is not configured");
            var password = _config["Smtp:Password"] ?? throw new InvalidOperationException("SMTP Password is not configured");
            if (!int.TryParse(portString, out int port))
                throw new InvalidOperationException("SMTP Port is not a valid integer");

            using var smtpClient = new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
            };

            var message = new MailMessage
            {
                From = new MailAddress(from),
                Subject = "Order Cancelled by User",
                Body = $"User '{userName}' has cancelled order #{orderId}.",
                IsBodyHtml = false
            };
            message.To.Add(staffEmail);

            await smtpClient.SendMailAsync(message);
        }


    }
}