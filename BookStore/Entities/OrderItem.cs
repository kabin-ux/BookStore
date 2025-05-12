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
        public decimal Price { get; set; }

        [Required]
<<<<<<< HEAD
        [ForeignKey("Order")]git 
=======
        [ForeignKey("Order")]
>>>>>>> 0a09c6996bea36f9536a19920a05d2b0f48dbaaa
        public int OrderId { get; set; }
        public Orders Order { get; set; }

        [Required]
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Books Book { get; set; }
    }
}