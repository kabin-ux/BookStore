using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Entities
{
    public class Whitelists
    {
        [Key]
        public int WhitelistId { get; set; }

        [Required]
        [EnumDataType(typeof(AvailabilityStatus))]
        public AvailabilityStatus IsAvailable { get; set; }

        [Required]
        public DateTime DateAdded { get; set; }

        [Required]
        [ForeignKey("User")]
        public long UserId { get; set; }
        public Users User { get; set; }

        [Required]
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Books Book { get; set; }
    }

    public enum AvailabilityStatus
    {
        Yes,
        No,
        LibraryAccessOnly
    }
}
