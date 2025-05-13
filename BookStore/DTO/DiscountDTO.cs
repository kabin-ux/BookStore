namespace BookStore.DTO
{
    public class DiscountDTO
    {
        public int DiscountId { get; set; }
        public int BookId { get; set; }
        public double DiscountPercent { get; set; }
        public decimal DiscountedPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsOnSale { get; set; }
    }
}
