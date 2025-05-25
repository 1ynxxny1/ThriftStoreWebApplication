using ThriftStoreWebApp.Models;

namespace ThriftStoreWebApp.Service.DTOs
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public Product Product { get; set; } = new();
    }
}
