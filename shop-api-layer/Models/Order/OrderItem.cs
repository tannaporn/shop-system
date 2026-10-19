using System.ComponentModel.DataAnnotations.Schema;

namespace shop_api_layer.Models.Order
{
    [Table("tb_order_items")]
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // shortcut: nav property added to fix EF join in DeleteProductCommand; no inverse configured
        public Order Order { get; set; } = null!;
    }
}
