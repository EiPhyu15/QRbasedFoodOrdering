using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRbasedFoodOrdering.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [ForeignKey("TableId")]
        public int TableId { get; set; }
        public Table Table { get; set; }
        public DateTime CreatedAt { get; set; }
        public string QRCode { get; set; } // This can be used to generate a QR code for the order
        public OrderStatus status { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
