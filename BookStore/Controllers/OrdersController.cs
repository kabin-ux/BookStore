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
