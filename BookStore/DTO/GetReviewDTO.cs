namespace BookStore.DTO
{
    public class GetReviewDTO
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public int BookId { get; set; }
        public long UserId { get; set; }

        public string UserName { get; set; }
    }
}