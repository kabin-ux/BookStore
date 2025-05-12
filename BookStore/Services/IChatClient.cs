using BookStore.DTO;

namespace BookStore.Services
{
    public interface IChatClient
    {
        Task ReceiveMessage(string message);
        Task ReceiveOrderNotification(OrderNotificationDTO notification);

    }
}