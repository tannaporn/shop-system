using System.ComponentModel.DataAnnotations.Schema;

namespace shop_api_layer.Models.Cart
{
    [Table("tb_carts")]
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<CartItem> Items { get; set; } = [];
    }
}
