namespace BookStore.Services
{
    public interface IEmailService
    {
        string GenerateCode();
        Task SendOrderConfirmationAsync(string code, string toEmail, long orderId, decimal billAmount, decimal finalAmount);
    }
}