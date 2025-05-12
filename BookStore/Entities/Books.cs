using System.ComponentModel.DataAnnotations;

namespace BookStore.Entities
{
    public class Books
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

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

        [Required]
        public bool IsStoreOnlyAccess { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        public DateTime PublicationDate { get; set; }

        [Required]
        public DateTime ArrivalDate { get; set; }

        public virtual Discounts Discount { get; set; }

        public virtual ICollection<Reviews> Reviews { get; set; }
    }
}
