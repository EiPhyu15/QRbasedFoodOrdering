using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QRbasedFoodOrdering.Models;
using QRbasedFoodOrdering.Controllers;

namespace QRbasedFoodOrdering.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<QRbasedFoodOrdering.Models.Table> Table { get; set; } = default!;
        public DbSet<QRbasedFoodOrdering.Models.Category> Category { get; set; } = default!;
        public DbSet<QRbasedFoodOrdering.Models.FoodItem> FoodItem { get; set; } = default!;
        public DbSet<QRbasedFoodOrdering.Models.Order> Order { get; set; } = default!;
        public DbSet<QRbasedFoodOrdering.Models.OrderDetail> OrderDetail { get; set; } = default!;
        public DbSet<QRbasedFoodOrdering.Models.CartItem> CartItem { get; set; } = default!;
        public DbSet<QRbasedFoodOrdering.Models.CartItem> Admin { get; set; } = default!;

    }
}
