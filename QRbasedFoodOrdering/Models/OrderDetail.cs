using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRbasedFoodOrdering.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public OrderDetailStatus Status { get; set; }
        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public Order Order { get; set; }
        [ForeignKey("FoodItemId")]
        public int FoodItemId { get; set; }
        public FoodItem FoodItem { get; set; }

    }
}
