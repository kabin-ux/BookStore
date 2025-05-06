namespace BookStore.DTO
{
    public class ReviewDTO
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public int BookId { get; set; }
    }
}
