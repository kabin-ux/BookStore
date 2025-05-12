using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Entities
{
    public class Orders
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string ClaimCode { get; set; }

        [Required]
        public decimal BillAmount { get; set; }

        [Required]
        public decimal DiscountApplied { get; set; }

        [Required]
        public decimal FinalAmount { get; set; }

        [Required]
        [ForeignKey("User")]
        public long UserId { get; set; }
        public Users User { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}