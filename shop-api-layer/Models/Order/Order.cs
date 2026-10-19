using System.ComponentModel.DataAnnotations.Schema;
using shop_api_layer.Enums;

namespace shop_api_layer.Models.Order
{
    [Table("tb_orders")]
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<OrderItem> Items { get; set; } = [];
    }
}
