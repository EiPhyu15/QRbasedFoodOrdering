using System.ComponentModel.DataAnnotations;

namespace QRbasedFoodOrdering.Models
{
    public class Table
    {
        [Key]
        public int TableId { get; set; }
        public string TableName { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
