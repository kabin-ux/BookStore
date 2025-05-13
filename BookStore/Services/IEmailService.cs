namespace BookStore.Services
{
    public interface IEmailService
    {
        string GenerateCode();
        Task SendOrderCancellationToStaffAsync(string email, string userName, int orderId);
        Task SendOrderConfirmationAsync(string code, string toEmail, long orderId, decimal billAmount, decimal finalAmount);
    }
}