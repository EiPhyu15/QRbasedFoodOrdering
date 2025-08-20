using System.ComponentModel.DataAnnotations;

namespace QRbasedFoodOrdering.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public ICollection<FoodItem> FoodItems { get; set; }
    }
}
