using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookStore.DTO;
using BookStore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public OrdersController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDTO>> CreateOrder(OrderCreateDTO orderDto)
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var order = new Orders
            {
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                ClaimCode = GenerateClaimCode(),
                BillAmount = orderDto.BillAmount,
                DiscountApplied = orderDto.DiscountApplied,
                FinalAmount = orderDto.FinalAmount,
                UserId = userId
            };

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

            var response = new OrderResponseDTO
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                ClaimCode = order.ClaimCode,
                BillAmount = order.BillAmount,
                DiscountApplied = order.DiscountApplied,
                FinalAmount = order.FinalAmount,
                OrderItems = orderDto.OrderItems
            };

            return Ok(response);
        }

        [HttpPost("{orderId}/cancel")]
        public async Task<ActionResult> CancelOrder(int orderId)
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound("Order not found");
            }

            if (order.Status == "Cancelled")
            {
                return BadRequest("Order is already cancelled");
            }

            if (order.Status == "Completed")
            {
                return BadRequest("Cannot cancel a completed order");
            }

            order.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order cancelled successfully" });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetUserOrders()
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
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

            return Ok(response);
        }

        private string GenerateClaimCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }
    }
} 