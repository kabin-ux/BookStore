using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Entities
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [Required]
        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public Carts Cart { get; set; }

        [Required]
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Books Book { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}