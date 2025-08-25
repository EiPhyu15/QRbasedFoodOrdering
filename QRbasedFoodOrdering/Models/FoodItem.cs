using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRbasedFoodOrdering.Models
{
    public class FoodItem
    {
        [Key]
        public int FoodItemId { get; set; }
        public string Title { get; set; }
        public string imageUrl { get; set; }
        public double Price { get; set; }
        public bool IsActive{ get; set; }
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }

    }
}
