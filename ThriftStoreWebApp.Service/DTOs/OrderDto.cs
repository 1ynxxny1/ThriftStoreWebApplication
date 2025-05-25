using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThriftStoreWebApp.Service.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;

        public List<OrderItemDto> Items { get; set; } = new();
        public decimal ShippingFee { get; set; }

        public string DeliveryAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentDetails { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ClientDto? Client { get; set; }
    }
}