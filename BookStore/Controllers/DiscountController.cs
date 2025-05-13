using BookStore.DTO;
using BookStore.Exceptions;
using BookStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Member")]
    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountsController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscount(DiscountCreateDTO dto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid request body");

            var result = await _discountService.CreateDiscountAsync(dto);
            return Ok(new BaseResponse<object>(200,true,"Created Discount",result));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDiscounts()
        {
            var result = await _discountService.GetAllDiscountsAsync();
            return Ok(new BaseResponse<object>(200,true," All Discounted Books",result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            await _discountService.DeleteDiscountAsync(id);
            return Ok(new BaseResponse<object>(200,true,"Discount deleted successfully"));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateDiscount(DiscountUpdateDTO dto)
        {
            var updated = await _discountService.UpdateDiscountAsync(dto);
            return Ok(new BaseResponse<object>(200, true, "Discount Updated successfully",updated));
        }

    }
}
