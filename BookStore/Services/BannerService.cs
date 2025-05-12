using Microsoft.EntityFrameworkCore;
using BookStore.DTOs;
using BookStore.Entities;

namespace BookStore.Services
{
    public class BannerService : IBannerService
    {
        private readonly ApplicationDBContext _context;

        public BannerService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BannerResponseDTO>> GetActiveBannersAsync()
        {
            var currentTime = DateTime.UtcNow;
            var banners = await _context.Banners
                .Include(b => b.CreatedByUser)
                .Where(b => b.IsActive && b.StartDate <= currentTime && b.EndDate >= currentTime)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return banners.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<BannerResponseDTO>> GetAllBannersAsync()
        {
            var banners = await _context.Banners
                .Include(b => b.CreatedByUser)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return banners.Select(MapToResponseDTO);
        }

        public async Task<BannerResponseDTO> GetBannerByIdAsync(int id)
        {
            var banner = await _context.Banners
                .Include(b => b.CreatedByUser)
                .FirstOrDefaultAsync(b => b.BannerId == id);

            if (banner == null)
                return null;

            return MapToResponseDTO(banner);
        }

        public async Task<BannerResponseDTO> CreateBannerAsync(CreateBannerDTO bannerDto, long userId)
        {
            var banner = new Banners
            {
                Title = bannerDto.Title,
                Description = bannerDto.Description,
                ImageUrl = bannerDto.ImageUrl,
                StartDate = bannerDto.StartDate,
                EndDate = bannerDto.EndDate,
                IsActive = true,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();

            return await GetBannerByIdAsync(banner.BannerId);
        }

        public async Task<BannerResponseDTO> UpdateBannerAsync(int id, UpdateBannerDTO bannerDto)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
                return null;

            banner.Title = bannerDto.Title;
            banner.Description = bannerDto.Description;
            banner.ImageUrl = bannerDto.ImageUrl;
            banner.StartDate = bannerDto.StartDate;
            banner.EndDate = bannerDto.EndDate;
            banner.IsActive = bannerDto.IsActive;

            await _context.SaveChangesAsync();

            return await GetBannerByIdAsync(id);
        }

        public async Task DeleteBannerAsync(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner != null)
            {
                _context.Banners.Remove(banner);
                await _context.SaveChangesAsync();
            }
        }

        private static BannerResponseDTO MapToResponseDTO(Banners banner)
        {
            return new BannerResponseDTO
            {
                BannerId = banner.BannerId,
                Title = banner.Title,
                Description = banner.Description,
                ImageUrl = banner.ImageUrl,
                StartDate = banner.StartDate,
                EndDate = banner.EndDate,
                IsActive = banner.IsActive,
                CreatedAt = banner.CreatedAt,
                CreatedByUserName = banner.CreatedByUser?.UserName
            };
        }
    }
} 