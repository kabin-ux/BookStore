using System.ComponentModel.DataAnnotations;

namespace BookStore.DTO
{
    // Yo sabai classes sabai seperate file hunu parcha hai subodh
    public class OrderCreateDTO
    {
        [Required]
        public List<OrderItemDTO> OrderItems { get; set; }
    }

    public class OrderItemDTO
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }
    }

    public class OrderResponseDTO
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string ClaimCode { get; set; }
        public decimal BillAmount { get; set; }
        public decimal DiscountApplied { get; set; }
        public decimal FinalAmount { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
    }

}

