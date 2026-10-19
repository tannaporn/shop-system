using System.ComponentModel.DataAnnotations.Schema;

namespace shop_api_layer.Models.Customer
{
    [Table("tb_customers")]
    public class Customer
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
