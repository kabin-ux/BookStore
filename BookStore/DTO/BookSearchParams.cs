namespace BookStore.DTO
{
    public class BookSearchParams
    {
        public string? Search { get; set; } = "";
        public string? Sort { get; set; } = "";
        public string? Author { get; set; } = "";
        public string? Genre { get; set; }
        public string? Format { get; set; }
        public string? Language { get; set; }
        public string? Availability { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? MinPrice { get; set; }
        public string? MaxPrice { get; set; }
        public string? Filter { get; set; } 
    }

}
