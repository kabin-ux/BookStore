using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Entities
{
    public class Carts
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Books Book { get; set; }

        [Required]
        [ForeignKey("User")]
        public long UserId { get; set; }
        public Users User { get; set; }
    }
}
