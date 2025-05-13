namespace BookStore.DTO
{
    public class DiscountCreateDTO
    {
        public int BookId { get; set; }
        public double DiscountPercent { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}