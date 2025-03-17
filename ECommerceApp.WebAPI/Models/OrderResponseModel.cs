namespace ECommerceApp.WebAPI.Models
{
    public class OrderResponseModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemModel> OrderItems { get; set; }
    }
}
