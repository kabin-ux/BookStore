using BookStore.DTO;
using BookStore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly ApplicationDBContext _context;
        private readonly IEmailService _emailService;
        private readonly ICartService _cartService;
        private string code;
        private decimal BillAmount;
        private decimal DiscountApplied;
        private decimal FinalAmount;

        public OrdersService(ApplicationDBContext context,IEmailService emailService, ICartService cartService)
        {
            _context = context;
            _emailService = emailService;
            _cartService = cartService;
        }

        public async Task<String> CreateOrder(OrderCreateDTO orderDto, long userId, string email)
        {
            code = _emailService.GenerateCode();

                foreach (var orderItem in orderDto.OrderItems)
            {
                await _cartService.RemoveQuantityFromCartAsync(userId, orderItem.BookId, orderItem.Quantity);
            }
            foreach (var orderItem in orderDto.OrderItems)
            {
                BillAmount = BillAmount + (orderItem.Quantity * orderItem.Price);
            }
            //Add Logic for discount eta ani final amount pani calculate gara
            // kei user lai response pathaune bhaye pani pathaye huncha eta bata
            // like discount ayo bhane yo message pathaune ani discount chaina bhane arko message pathaune in toaster
            var order = new Orders
            {
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                ClaimCode = code,
                BillAmount = BillAmount,
                DiscountApplied = DiscountApplied,
                FinalAmount = FinalAmount,
                UserId = userId
            };
            await _emailService.SendEmailAsync(code, email);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderItems = orderDto.OrderItems.Select(item => new OrderItem
            {
                OrderId = order.OrderId,
                BookId = item.BookId,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList();

            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();

            return ("Your Order Placed!");
        }

        public async Task<String> CancelOrder(int orderId, long userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                 throw new Exception("Order not found");
            }

            if (order.Status == "Cancelled")
            {
                throw new Exception("Order is already cancelled");
            }

            if (order.Status == "Completed")
            {
                throw new Exception("Cannot cancel a completed order");
            }

            order.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return "Order cancelled successfully";
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetUserOrders(long userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ToListAsync();

            var response = orders.Select(o => new OrderResponseDTO
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                ClaimCode = o.ClaimCode,
                BillAmount = o.BillAmount,
                DiscountApplied = o.DiscountApplied,
                FinalAmount = o.FinalAmount,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    BookId = oi.BookId,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            });

            return response;
        }

        private string GenerateClaimCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }
    }
}
