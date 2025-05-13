namespace BookStore.DTO
{
    public class DiscountUpdateDTO
    {
        public int DiscountId { get; set; }
        public double DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
