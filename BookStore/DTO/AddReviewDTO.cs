namespace BookStore.DTO
{
    public class AddReviewDTO
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public int BookId { get; set; }
    }
}