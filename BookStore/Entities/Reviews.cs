using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Entities
{
    public class Reviews
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; } 

        [Required]
        public DateTime Date { get; set; }

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
