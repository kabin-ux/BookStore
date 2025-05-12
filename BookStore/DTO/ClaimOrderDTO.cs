using System.ComponentModel.DataAnnotations;

namespace BookStore.DTO
{
    public class ClaimOrderDTO
    {
        [Required]
        public string ClaimCode { get; set; }
    }
}