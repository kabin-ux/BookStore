
namespace BookStore.Services
{
    public interface IEmailService
    {
        string GenerateCode();
        Task SendEmailAsync(string code, string toEmail);
    }
}
