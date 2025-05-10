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
   
    }
}
