using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRbasedFoodOrdering.Data;
using QRbasedFoodOrdering.Models;

namespace QRbasedFoodOrdering.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> WeeklyFoodReport()
        {
            var startofWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            var endofWeek = startofWeek.AddDays(7);
            var reportData = await _context.OrderDetail.Include(od => od.Order).Include(od => od.FoodItem).Where(od => od.Order.CreatedAt >= startofWeek && od.Order.CreatedAt < endofWeek).GroupBy(od => new
            {
                Day = od.Order.CreatedAt.DayOfWeek,
                Food = od.FoodItem.Title
            }).Select(g => new WeeklyFoodReportViewModel
            {
                Day = g.Key.Day.ToString(),
                FoodTitle = g.Key.Food,
                TotalQuantity = g.Sum(od => od.Quantity)
            }).ToListAsync();
            
               
 
            return View(reportData);
        }


    }
}
