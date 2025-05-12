<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookStore.DTO;
using BookStore.Entities;
using Microsoft.EntityFrameworkCore;
=======
﻿using BookStore.DTO;
using BookStore.Entities;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
>>>>>>> 0a09c6996bea36f9536a19920a05d2b0f48dbaaa
using System.Security.Claims;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
<<<<<<< HEAD
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
=======
    public class OrdersController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly IOrdersService _ordersService;

        public OrdersController(UserManager<Users> userManager, IOrdersService ordersService)
        {
            _userManager = userManager;
            _ordersService = ordersService;
        }

        [HttpPost("/add")]
        //Eta Member lekhnu parcha parcha hola hai mero ma User matra cha (kabin lai bhaneko)
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<OrderResponseDTO>> CreateOrder(OrderCreateDTO orderDto)
        {
            //Subodh ko ma kasari leko tha bhayena hai i just took user information from the token 
            // esari mero cart ra Whitelist ma gareko thiye so esari nai thik huncha jasto lagyo
            var user = await _userManager.GetUserAsync(User);
            //yo null check gareko ma euta method banaune ki ? or like euta class banaune ani jaile check garne because yo sabai controller ma halnu parcha its alright if we dont want to do
            // just Marks dherai paucha ki code resusability le gardaa marks count huncha 

            if (user == null)
                return Unauthorized(new BaseResponse<string>(401, false, "Unauthorized"));
            var userId = user.Id;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return NotFound(new BaseResponse<string>(404, false, "Email Not Found"));
            try
            {
                String message = await _ordersService.CreateOrder(orderDto, userId, email);
                //yo order ma multiple sucessfull response pathaune bhayera message bhanera pass gareko natra you can see cart or whitelist ma tesma chai only one sucessful message
                return Ok(new BaseResponse<string>(200, true, message));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<string>(400, false, ex.Message));
            }
        }

        [HttpPut("{orderId}/cancel")]
        //Eta Member lekhnu parcha parcha hola hai mero ma User matra cha (kabin lai bhaneko)
        [Authorize(Roles = "Member")]
        public async Task<ActionResult> CancelOrder(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new BaseResponse<string>(401, false, "Unauthorized"));
            var userId = user.Id;
            try
            {
                String message = await _ordersService.CancelOrder(orderId, userId);
                return Ok(new BaseResponse<Object>(200, true, message));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<string>(400, false, ex.Message));
            }

        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetUserOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new BaseResponse<string>(401, false, "Unauthorized"));
            var userId = user.Id;


            try
            {
                var orders = await _ordersService.GetUserOrders(userId);

                return Ok(new BaseResponse<Object>(200, true, "Generated User Orders", orders));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<string>(400, false, ex.Message));
            }
        }
    }
}
>>>>>>> 0a09c6996bea36f9536a19920a05d2b0f48dbaaa
