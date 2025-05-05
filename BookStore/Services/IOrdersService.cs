using BookStore.DTO;
using BookStore.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Services
{
    public interface IOrdersService
    {
        Task<String> CreateOrder(OrderCreateDTO orderDto, long userId, string email);
        Task<String> CancelOrder(int orderId, long userId);
        Task<IEnumerable<OrderResponseDTO>> GetUserOrders(long userId);
    }
}