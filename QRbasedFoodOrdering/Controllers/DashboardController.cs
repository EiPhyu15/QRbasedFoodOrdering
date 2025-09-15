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
                     TotalSales = g.Sum(o => o.OrderDetails.Sum(d => d.Quantity * d.Price))

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


        
        public async Task<IActionResult> PopularFoodsByWeek(int topN = 5)
        {
            var query = _context.OrderDetail
                .Include(od => od.Order)
                .Include(od => od.FoodItem)
                .AsEnumerable()
                .GroupBy(od =>
                {
                    var date = od.Order.CreatedAt.Date;

                    // Calculate Monday of the week
                    var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
                    var weekStart = date.AddDays(-diff);
                    var weekEnd = weekStart.AddDays(6);

                    return new { WeekStart = weekStart, WeekEnd = weekEnd, od.FoodItemId, od.FoodItem.Title };
                })
                .Select(g => new PopularFoodReportViewModel
                {
                    StartDate = g.Key.WeekStart,
                    EndDate = g.Key.WeekEnd,
                    FoodItemId = g.Key.FoodItemId,
                    FoodItemName = g.Key.Title,
                    TotalQuantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            // Pick Top N per week
            var result = query
                .GroupBy(q => new { q.StartDate, q.EndDate })
                .SelectMany(g => g
                    .OrderByDescending(x => x.TotalQuantity)
                    .Take(topN))
                .OrderBy(r => r.StartDate)
                .ToList();

            return View(result);
        }

        public async Task<IActionResult> FoodOrdersByDay()
        {
            var rawData = _context.OrderDetail
                .Include(od => od.Order)
                .Include(od => od.FoodItem)
                .AsEnumerable()
                .GroupBy(od => new
                {
                    DayOfWeek = od.Order.CreatedAt.DayOfWeek,
                    od.FoodItem.Title
                })
                .Select(g => new
                {
                    DayOfWeek = g.Key.DayOfWeek,
                    FoodName = g.Key.Title,
                    TotalQuantity = g.Sum(x => x.Quantity)
                })
                .ToList();

            // Order days manually Monday → Sunday
            var days = Enum.GetValues(typeof(DayOfWeek))
                           .Cast<DayOfWeek>()
                           .Select(d => d.ToString())
                           .ToList();

            // Food items
            var foodNames = rawData.Select(x => x.FoodName).Distinct().ToList();

            // Build dataset
            var chartData = foodNames.Select(food => new
            {
                label = food,
                data = days.Select(day =>
                    rawData.Where(x => x.FoodName == food && x.DayOfWeek.ToString() == day)
                           .Sum(x => x.TotalQuantity)
                ).ToList()
            });

            ViewBag.Days = days;
            ViewBag.ChartData = chartData;

            return View();
        }





    }

}





