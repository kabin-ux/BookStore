using System.ComponentModel.DataAnnotations;

namespace BookStore.DTO
{
    public class BookCreateUpdateDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public string Publisher { get; set; }

        [Required]
        public string Format { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public bool IsAvailable { get; set; }
    }
}
