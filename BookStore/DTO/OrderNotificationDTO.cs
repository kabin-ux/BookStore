namespace BookStore.DTO
{
    public class OrderNotificationDTO
    {
        public string UserName { get; set; }
        public List<OrderItemNotificationDTO> Items { get; set; }
        public DateTime OrderDate { get; set; }
    }
}