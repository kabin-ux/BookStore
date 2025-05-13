using BookStore.Entities;

namespace BookStore.Helper
{
    public static class DiscountHelper
    {
        public static Discounts? GetActiveDiscount(IEnumerable<Discounts> discounts)
        {
            var now = DateTime.UtcNow;
            return discounts.FirstOrDefault(d => d.StartDate <= now && d.EndDate >= now);
        }
    }
}
