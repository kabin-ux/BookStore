using BookStore.Services;
using Microsoft.AspNetCore.SignalR;

namespace BookStore.WebSocket
{
    public sealed class ChatHub : Hub<IChatClient>
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.ReceiveMessage($"{Context.ConnectionId} has joined");
        }

        public async Task sendMessage(string message)
        {
            await Clients.All.ReceiveMessage($"{Context.ConnectionId}: ${message}");
        }
    }
}
