using BookStore.DTO;
using BookStore.Entities;
using BookStore.Exceptions;
using BookStore.Helper;
using BookStore.WebSocket;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly ApplicationDBContext _context;
        private readonly IEmailService _emailService;
        private readonly ICartService _cartService;
        private readonly IHubContext<ChatHub, IChatClient> _hubContext;

        public OrdersService(ApplicationDBContext context, IEmailService emailService, ICartService cartService, IHubContext<ChatHub, IChatClient> hubContext)
        {
            _context = context;
            _emailService = emailService;
            _cartService = cartService; 
            _hubContext = hubContext;
        }

        public async Task<string> CreateOrder(OrderCreateDTO orderDto, long userId, string email)
        {
            await ValidateItemsInCartAsync(userId, orderDto.OrderItems);

            if (string.IsNullOrEmpty(email))
                throw new ValidationException("Email address is required");

            if (!IsValidEmail(email))
                throw new ValidationException("Invalid email address format");

            var code = _emailService.GenerateCode();
            string discountMessage = "";

            Dictionary<long, decimal> bookPrices = new();
            decimal billAmount = 0;

            foreach (var orderItem in orderDto.OrderItems)
            {
                var book = await _context.Books
                    .Include(b => b.Discounts)
                    .FirstOrDefaultAsync(b => b.BookId == orderItem.BookId);

                if (book == null)
                    throw new NotFoundException($"Book with ID {orderItem.BookId} not found");

                var activeDiscount = DiscountHelper.GetActiveDiscount(book.Discounts);
                var finalPrice = activeDiscount != null && activeDiscount.IsOnSale
                    ? activeDiscount.DiscountedPrice
                    : book.Price;

                billAmount += orderItem.Quantity * finalPrice;
                bookPrices[orderItem.BookId] = finalPrice;
            }

            int totalBooks = orderDto.OrderItems.Sum(item => item.Quantity);

            int totalOrdersCount = await _context.Orders
                .CountAsync(o => o.UserId == userId);

            decimal quantityDiscount = 0;
            decimal loyaltyDiscount = 0;

            if (totalBooks >= 5)
            {
                quantityDiscount = Math.Round(billAmount * 0.05m, 2);
                discountMessage += "For ordering 5+ books, a 5% discount is applied. ";
            }

            if (totalOrdersCount > 0 && (totalOrdersCount + 1) % 11 == 0)
            {
                loyaltyDiscount = Math.Round(billAmount * 0.10m, 2);
                discountMessage += $"This is your {totalOrdersCount + 1}th order — a 10% loyalty discount is applied. ";
            }

            decimal totalDiscount = quantityDiscount + loyaltyDiscount;
            decimal finalAmount = billAmount - totalDiscount;

            // Clear quantity from cart
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

            string responseMessage = $"Order successfully placed! Bill Amount: Rs.{billAmount:F2}";
            if (totalDiscount > 0)
            {
                responseMessage += $" {discountMessage}Total discount: Rs.{totalDiscount:F2}. Final amount: Rs.{finalAmount:F2}";
            }

            await _emailService.SendOrderConfirmationAsync(code, email, order.OrderId, billAmount, finalAmount);

            return responseMessage;
        }


        public async Task<String> CancelOrder(int orderId, long userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                throw new NotFoundException("Order not found");
            }

            if (order.Status == "Cancelled")
            {
                throw new ForbiddenException("Order is already cancelled");
            }

            if (order.Status == "Completed")
            {
                throw new ForbiddenException("Cannot cancel a completed order");
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

            if (!orders.Any())
            {
                throw new NotFoundException("No orders found for this user");
            }

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
        public async Task<OrderResponseDTO> ProcessClaimCode(ClaimOrderDTO claimOrderDto)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.ClaimCode == claimOrderDto.ClaimCode);

            if (order == null)
            {
                throw new ValidationException("Invalid claim code");
            }

            if (order.Status == "Completed")
            {
                throw new ValidationException("Order has already been claimed");
            }

            if (order.Status == "Cancelled")
            {
                throw new ForbiddenException("Cannot claim a cancelled order");
            }
            foreach (var orderItem in order.OrderItems)
            {
                if (orderItem.Quantity > 0 && orderItem.Book != null)
                {
                    if (orderItem.Book.StockQuantity < orderItem.Quantity)
                    {
                        throw new ValidationException($"Not enough stock for book: {orderItem.Book.Title}");
                    }

                    orderItem.Book.StockQuantity -= orderItem.Quantity;

                    if (orderItem.Book.StockQuantity == 0)
                    {
                        orderItem.Book.IsAvailable = false;
                    }
                }
            }

            order.Status = "Completed";
            await _context.SaveChangesAsync();

            // Re-fetch order with complete navigation properties for safe SignalR notification
            var fullOrder = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

            if (fullOrder != null)
            {
                await BroadCastOrderNotification(fullOrder);
            }

            return new OrderResponseDTO
            {
                UserId = order.UserId,
                UserName = order.User.UserName,
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                ClaimCode = order.ClaimCode,
                BillAmount = order.BillAmount,
                DiscountApplied = order.DiscountApplied,
                FinalAmount = order.FinalAmount,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    BookId = oi.BookId,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    BookTitle = oi.Book.Title
                }).ToList()
            };
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public async Task<string> DeleteOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new NotFoundException("Order not found");
            }

            if (order.Status == "Completed")
            {
                throw new ForbiddenException("Completed orders cannot be deleted");
            }

            _context.OrderItems.RemoveRange(order.OrderItems);

            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            return "Order deleted successfully";
        }
        private async Task ValidateItemsInCartAsync(long userId, List<OrderItemDTO> orderItems)
        {
            foreach (var orderItem in orderItems)
            {
                var cartItem = await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == orderItem.BookId);

                if (cartItem == null)
                {
                    throw new NotFoundException($"Book with ID {orderItem.BookId} not found in your cart");
                }

                if (cartItem.Quantity < orderItem.Quantity)
                {
                    throw new ValidationException($"Requested quantity ({orderItem.Quantity}) for book ID {orderItem.BookId} exceeds available quantity ({cartItem.Quantity}) in your cart");
                }
            }
        }
        private async Task BroadCastOrderNotification(Orders order)
        {
            var notification = new OrderNotificationDTO
            {
                UserName = order.User.UserName,
                OrderDate = order.OrderDate,
                Items = order.OrderItems.Select(i => new OrderItemNotificationDTO
                {
                    BookTitle = i.Book.Title,
                    Quantity = i.Quantity
                }).ToList()
            };

            // Use the SendOrderNotification method of the hub
            await _hubContext.Clients.All.ReceiveOrderNotification(notification);
        }
    }
}