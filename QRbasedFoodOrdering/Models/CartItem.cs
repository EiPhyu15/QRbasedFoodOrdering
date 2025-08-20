using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRbasedFoodOrdering.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        public string CartId { get; set; } // This can be used to track the cart across sessions or users
        public int Quantity { get; set; }
        public double Price { get; set; }
        public int FoodItemId { get; set; }
        [ForeignKey("FoodItemId")]
        public FoodItem FoodItem { get; set; }
        public int TableId { get; set; }
        [ForeignKey("TableId")]
        public Table Table { get; set; }
    }
}
