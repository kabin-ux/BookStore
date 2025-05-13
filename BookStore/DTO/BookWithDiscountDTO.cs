using BookStore.DTO;

namespace BookStore.DTOs
{
    public class BookWithDiscountDTO
    {
        public int BookId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string Genre { get; set; }

        public string Language { get; set; }

        public string Publisher { get; set; }

        public string Format { get; set; }

        public string ISBN { get; set; }

        public int StockQuantity { get; set; }

        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsStoreOnlyAccess { get; set; }

        public string? ImagePath { get; set; }

        public DateTime PublicationDate { get; set; }

        public DateTime ArrivalDate { get; set; }

        public DiscountDTO? ActiveDiscount { get; set; }
    }
}