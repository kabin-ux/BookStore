using BookStore.DTO;
using BookStore.Entities;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<OrderResponseDTO>> CreateOrder(OrderCreateDTO orderDto)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            
            string message = await _ordersService.CreateOrder(orderDto, userId, email);
            return Ok(new BaseResponse<string>(200, true, message));
        }

        [HttpPut("{orderId}/cancel")]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult> CancelOrder(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            
            string message = await _ordersService.CancelOrder(orderId, userId);
            return Ok(new BaseResponse<object>(200, true, message));
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<ActionResult<IEnumerable<OrderResponseDTO>>> GetUserOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            
            var orders = await _ordersService.GetUserOrders(userId);
            return Ok(new BaseResponse<object>(200, true, "Generated User Orders", orders));
        }
    }
}