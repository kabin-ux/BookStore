using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BookStore.DTOs;
using BookStore.Services;

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
        public async Task<ActionResult<IEnumerable<BannerResponseDTO>>> GetActiveBanners()
        {
            var banners = await _bannerService.GetActiveBannersAsync();
            return Ok(banners);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<BannerResponseDTO>>> GetAllBanners()
        {
            var banners = await _bannerService.GetAllBannersAsync();
            return Ok(banners);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BannerResponseDTO>> GetBanner(int id)
        {
            var banner = await _bannerService.GetBannerByIdAsync(id);
            if (banner == null)
            {
                return NotFound();
            }
            return Ok(banner);
        }

        [HttpPost]
        public async Task<ActionResult<BannerResponseDTO>> CreateBanner(CreateBannerDTO bannerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var banner = await _bannerService.CreateBannerAsync(bannerDto, userId);
            return CreatedAtAction(nameof(GetBanner), new { id = banner.BannerId }, banner);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBanner(int id, UpdateBannerDTO bannerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var banner = await _bannerService.UpdateBannerAsync(id, bannerDto);
            if (banner == null)
            {
                return NotFound();
            }

            return Ok(banner);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            await _bannerService.DeleteBannerAsync(id);
            return NoContent();
        }
    }
} 