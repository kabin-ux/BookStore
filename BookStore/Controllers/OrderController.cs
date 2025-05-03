using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookStore.Entities;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public OrderController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            // Get user's cart
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return BadRequest("Cart is empty");
            }

            // Calculate total amount and apply discounts
            decimal totalAmount = cart.CartItems.Sum(ci => ci.Book.Price * ci.Quantity);
            decimal discountAmount = 0;

            // Apply 5% discount for 5 or more books
            if (cart.CartItems.Sum(ci => ci.Quantity) >= 5)
            {
                discountAmount += totalAmount * 0.05m;
            }

            // Check for 10% discount from successful orders
            var successfulOrders = await _context.Orders
                .CountAsync(o => o.UserId == userId && o.Status == "Completed");
            
            if (successfulOrders > 0 && successfulOrders % 10 == 0)
            {
                discountAmount += totalAmount * 0.10m;
            }

            // Create new order
            var order = new Orders
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                ClaimCode = GenerateClaimCode(),
                BillAmount = totalAmount,
                DiscountApplied = discountAmount,
                FinalAmount = totalAmount - discountAmount,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    BookId = ci.BookId,
                    Quantity = ci.Quantity,
                    Price = ci.Book.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            
            // Clear the cart
            _context.CartItems.RemoveRange(cart.CartItems);
            
            await _context.SaveChangesAsync();

            return Ok(new { 
                OrderId = order.OrderId,
                ClaimCode = order.ClaimCode,
                FinalAmount = order.FinalAmount
            });
        }

        [HttpDelete("cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound("Order not found");
            }

            if (order.Status != "Pending")
            {
                return BadRequest("Only pending orders can be cancelled");
            }

            order.Status = "Cancelled";
            order.CancelledAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order cancelled successfully" });
        }

        private string GenerateClaimCode()
        {
            // Generate a random 6-character alphanumeric code
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
} 