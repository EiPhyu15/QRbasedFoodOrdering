using System.Globalization;
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
        public async Task<IActionResult> DailySales()
        {
            var dailySales = await _context.Order
                 .Include(o => o.OrderDetails)
                 .Where(o => o.status == OrderStatus.Completed) 
                 .GroupBy(o => o.CreatedAt.Date)
                 .Select(g => new DailySalesViewModel
                 {
                     Date = g.Key,
                     //TotalOrders = g.Count(),
                     TotalOrders = g.Select(x => x.OrderId).Distinct().Count(),
                     TotalSales = g.Sum(o => o.OrderDetails.Sum(d => d.Quantity *d.Price))
                     
                 })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return View(dailySales);
        }
        public async Task<IActionResult> WeeklySales()
        {
            var calendar = CultureInfo.CurrentCulture.Calendar;
            var weeklysales = _context.Order
                .Include(o => o.OrderDetails)
                .Where(o => o.status == OrderStatus.Completed)
                .AsEnumerable()
                .GroupBy(o => new
                {
                    Year = o.CreatedAt.Year,
                    Week = calendar.GetWeekOfYear(o.CreatedAt, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                })
                .Select(g => new WeeklySalesViewModel
                {
                    Year = g.Key.Year,
                    Week = g.Key.Week,
                    TotalOrders = g.Select(x => x.OrderId).Distinct().Count(),
                    //TotalOrders = g.Count(),
                    TotalSales = g.Sum(o => o.OrderDetails.Sum(d => d.Quantity * d.Price))
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Week)
                .ToList();
            return View(weeklysales);




        }


    }
}
