using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Entities
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Orders Order { get; set; }

        [Required]
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Books Book { get; set; }
    }
}
