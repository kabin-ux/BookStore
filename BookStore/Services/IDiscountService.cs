using BookStore.DTO;

namespace BookStore.Services
{
    public interface IDiscountService
    {
        Task<DiscountDTO> CreateDiscountAsync(DiscountCreateDTO dto);
        Task<List<DiscountDTO>> GetAllDiscountsAsync();
        Task<bool> DeleteDiscountAsync(int id);
        Task<DiscountDTO> UpdateDiscountAsync(DiscountUpdateDTO dto);

    }
}