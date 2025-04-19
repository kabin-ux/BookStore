using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Entities
{
    public class Discounts
    {
        [Key]
        public int DiscountId { get; set; }

        [Required]
        [Range(0, 100)]
        public double DiscountPercent { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public bool IsOnSale { get; set; }

        [Required]
        [ForeignKey("Books")]
        public int BookId { get; set; }
        public virtual Books Books { get; set; }
    }
}
