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

        public OrdersService(ApplicationDBContext context, IEmailService emailService, ICartService cartService)
        {
            _context = context;
            _emailService = emailService;
            _cartService = cartService;
        }

        public async Task<String> CreateOrder(OrderCreateDTO orderDto, long userId, string email)
        {
            var code = _emailService.GenerateCode();
            string discountMessage = "";

            // Calculate initial bill amount using current book prices from database
            Dictionary<long, decimal> bookPrices = new();
            decimal billAmount = 0;
            foreach (var orderItem in orderDto.OrderItems)
            {
                var book = await _context.Books.FindAsync(orderItem.BookId);
                if (book == null)
                {
                    throw new Exception($"Book with ID {orderItem.BookId} not found");
                }
                billAmount += (orderItem.Quantity * book.Price);
                bookPrices[orderItem.BookId] = book.Price;
            }

            // Calculate total quantity of books in the order
            int totalBooks = orderDto.OrderItems.Sum(item => item.Quantity);

            // Get completed orders count for the user
            int completedOrdersCount = await _context.Orders
                .CountAsync(o => o.UserId == userId && o.Status == "Completed");

            // Calculate discounts
            decimal quantityDiscount = 0;
            decimal loyaltyDiscount = 0;

            // 5% discount for ordering 5 or more books
            if (totalBooks >= 5)
            {
                quantityDiscount = Math.Round(billAmount * 0.05m, 2);
                discountMessage += "For ordering 5+ books, a 5% discount is applied. ";
            }

            // 10% loyalty discount for every 11th completed order (i.e., 11th, 22nd, 33rd...)
            int upcomingOrderNumber = completedOrdersCount + 1;
            if (upcomingOrderNumber % 11 == 0)
            {
                loyaltyDiscount = Math.Round(billAmount * 0.10m, 2);
                discountMessage += $"This is your {upcomingOrderNumber}th order — a 10% loyalty discount is applied. ";
            }


            // Calculate total discount and final amount
            decimal totalDiscount = quantityDiscount + loyaltyDiscount;
            decimal finalAmount = billAmount - totalDiscount;

            // Remove items from cart
            foreach (var orderItem in orderDto.OrderItems)
            {
                await _cartService.RemoveQuantityFromCartAsync(userId, orderItem.BookId, orderItem.Quantity);
            }

            var order = new Orders
            {
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                ClaimCode = code,
                BillAmount = billAmount,
                DiscountApplied = totalDiscount,
                FinalAmount = finalAmount,
                UserId = userId
            };

            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("Generated code was null", nameof(code));

            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email is null", nameof(email));

            await _emailService.SendEmailAsync(code, email);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();



            var orderItems = orderDto.OrderItems.Select(item => new OrderItem
            {
                OrderId = order.OrderId,
                BookId = item.BookId,
                Quantity = item.Quantity,
                Price = bookPrices[item.BookId]
            }).ToList();

            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();

            string responseMessage = $"Order successfully placed! Bill Amount: ${billAmount:F2}";
            if (totalDiscount > 0)
            {
                responseMessage += $" {discountMessage}Total discount: ${totalDiscount:F2}. Final amount: ${finalAmount:F2}";
            }

            return responseMessage;
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
                .Include(o => o.User) // Include the User navigation property
                .Include(o => o.OrderItems)
                .ToListAsync();

            var response = orders.Select(o => new OrderResponseDTO
            {
                UserId = o.UserId,
                UserName = o.User.UserName,
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

    }

}