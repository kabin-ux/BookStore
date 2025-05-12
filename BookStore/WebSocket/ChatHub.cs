using BookStore.DTO;
using BookStore.Services;
using Microsoft.AspNetCore.SignalR;

namespace BookStore.WebSocket
{
    public sealed class ChatHub : Hub<IChatClient>
    {
        public async Task SendNotification(string message)
        {
            await Clients.All.ReceiveMessage($"just bought {message}!");
        }
        public async Task SendOrderNotification(OrderNotificationDTO notification)
        {
            await Clients.All.ReceiveOrderNotification(notification);
        }

    }
}