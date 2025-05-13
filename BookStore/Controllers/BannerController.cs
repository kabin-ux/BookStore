// BannerController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BookStore.DTO;
using BookStore.Services;
using BookStore.Exceptions;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class BannerController : ControllerBase
    {
        private readonly IBannerService _bannerService;

        public BannerController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<BaseResponse<IEnumerable<BannerResponseDTO>>>> GetActiveBanners()
        {
            var banners = await _bannerService.GetActiveBannersAsync();
            return Ok(new BaseResponse<IEnumerable<BannerResponseDTO>>(200, true, "Active banners fetched", banners));
        }

        [HttpGet("all")]
        public async Task<ActionResult<BaseResponse<IEnumerable<BannerResponseDTO>>>> GetAllBanners()
        {
            var banners = await _bannerService.GetAllBannersAsync();
            return Ok(new BaseResponse<IEnumerable<BannerResponseDTO>>(200, true, "All banners fetched", banners));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponse<BannerResponseDTO>>> GetBanner(int id)
        {
            var banner = await _bannerService.GetBannerByIdAsync(id);
            return Ok(new BaseResponse<BannerResponseDTO>(200, true, "Banner fetched", banner));
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<BannerResponseDTO>>> CreateBanner(CreateBannerDTO bannerDto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid banner input");

            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var banner = await _bannerService.CreateBannerAsync(bannerDto, userId);

            return Ok(new BaseResponse<BannerResponseDTO>(201, true, "Banner created", banner));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponse<BannerResponseDTO>>> UpdateBanner(int id, UpdateBannerDTO bannerDto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid banner update input");

            var banner = await _bannerService.UpdateBannerAsync(id, bannerDto);
            return Ok(new BaseResponse<BannerResponseDTO>(200, true, "Banner updated", banner));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteBanner(int id)
        {
            await _bannerService.DeleteBannerAsync(id);
            return Ok(new BaseResponse<string>(200, true, "Banner deleted successfully", null));
        }
    }
}
