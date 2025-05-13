using BookStore.DTO;
using BookStore.Entities;

namespace BookStore.Services
{
    public interface IBannerService
    {
        Task<IEnumerable<BannerResponseDTO>> GetActiveBannersAsync();
        Task<IEnumerable<BannerResponseDTO>> GetAllBannersAsync();
        Task<BannerResponseDTO> GetBannerByIdAsync(int id);
        Task<BannerResponseDTO> CreateBannerAsync(CreateBannerDTO bannerDto, long userId);
        Task<BannerResponseDTO> UpdateBannerAsync(int id, UpdateBannerDTO bannerDto);
        Task DeleteBannerAsync(int id);
    }
}