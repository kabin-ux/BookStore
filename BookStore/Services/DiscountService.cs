using BookStore.DTO;
using BookStore.Entities;
using BookStore.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly ApplicationDBContext _context;

        public DiscountService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<DiscountDTO> CreateDiscountAsync(DiscountCreateDTO dto)
        {
            var book = await _context.Books.FindAsync(dto.BookId);
            if (book == null)
                throw new NotFoundException("Book not found");
            var expiredDiscounts = await _context.Discounts
                .Where(d => d.BookId == dto.BookId && d.EndDate <= DateTime.UtcNow)
                .ToListAsync();
            _context.Discounts.RemoveRange(expiredDiscounts);
            await _context.SaveChangesAsync();
            var hasActiveDiscount = await _context.Discounts
            .AnyAsync(d => d.BookId == dto.BookId && d.EndDate > DateTime.UtcNow);

            if (hasActiveDiscount)
                throw new ConflictException("A valid discount already exists for this book");


            if (dto.DiscountPercent <= 0 || dto.DiscountPercent >= 100)
                throw new ValidationException("Discount percent must be between 0 and 100");

            if (dto.StartDate >= dto.EndDate)
                throw new ValidationException("Start date must be before end date");
            var discountFraction = (decimal)(dto.DiscountPercent / 100);
            var discountedPrice = Math.Round(book.Price * (1 - discountFraction), 2);

            var discount = new Discounts
            {
                BookId = dto.BookId,
                DiscountPercent = dto.DiscountPercent,
                DiscountedPrice = discountedPrice,
                StartDate = dto.StartDate.ToUniversalTime(),
                EndDate = dto.EndDate.ToUniversalTime()
            };

            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();

            return new DiscountDTO
            {
                DiscountId = discount.DiscountId,
                BookId = discount.BookId,
                DiscountPercent = discount.DiscountPercent,
                DiscountedPrice = discount.DiscountedPrice,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                IsOnSale = discount.IsOnSale
            };
        }

        public async Task<List<DiscountDTO>> GetAllDiscountsAsync()
        {
            var discounts = await _context.Discounts
                .Include(d => d.Books)
                .ToListAsync();

            return discounts.Select(d => new DiscountDTO
            {
                DiscountId = d.DiscountId,
                BookId = d.BookId,
                BookName = d.Books.Title,
                DiscountPercent = d.DiscountPercent,
                DiscountedPrice = d.DiscountedPrice,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                IsOnSale = d.IsOnSale
            }).ToList();
        }

        public async Task<bool> DeleteDiscountAsync(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
                throw new NotFoundException("Discount not found");

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<DiscountDTO> UpdateDiscountAsync(DiscountUpdateDTO dto)
        {
            var discount = await _context.Discounts
                .Include(d => d.Books)
                .FirstOrDefaultAsync(d => d.DiscountId == dto.DiscountId);

            if (discount == null)
                throw new NotFoundException("Discount not found");

            if (dto.DiscountPercent <= 0 || dto.DiscountPercent >= 100)
                throw new ValidationException("Discount percent must be between 0 and 100");

            if (dto.StartDate >= dto.EndDate)
                throw new ValidationException("Start date must be before end date");

            var discountFraction = (decimal)(dto.DiscountPercent / 100);
            var newDiscountedPrice = Math.Round(discount.Books.Price * (1 - discountFraction), 2);

            discount.DiscountPercent = dto.DiscountPercent;
            discount.StartDate = dto.StartDate.ToUniversalTime();
            discount.EndDate = dto.EndDate.ToUniversalTime();
            discount.DiscountedPrice = newDiscountedPrice;

            await _context.SaveChangesAsync();

            return new DiscountDTO
            {
                DiscountId = discount.DiscountId,
                BookId = discount.BookId,
                DiscountPercent = discount.DiscountPercent,
                DiscountedPrice = discount.DiscountedPrice,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                IsOnSale = discount.IsOnSale
            };
        }

    }
}