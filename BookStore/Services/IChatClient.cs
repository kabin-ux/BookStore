namespace BookStore.Services
{
    public interface IChatClient
    {
        Task ReceiveMessage(string message);
    }
}
