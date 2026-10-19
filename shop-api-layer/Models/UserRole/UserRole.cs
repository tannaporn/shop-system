using System.ComponentModel.DataAnnotations.Schema;

namespace shop_api_layer.Models.UserRole
{
    [Table("tb_user_roles")]
    public class UserRole
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
