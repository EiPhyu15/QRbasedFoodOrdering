using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRbasedFoodOrdering.Data;
using QRbasedFoodOrdering.Models;

namespace QRbasedFoodOrdering.Controllers
{
    public class KitchenController : Controller

    {
        private readonly ApplicationDbContext _context;
        public KitchenController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> MarkPaid(int orderId)
        {
            var order = await _context.Order.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            // Update the order status to Paid
            order.status = Models.OrderStatus.Completed;
            _context.Order.Update(order);
            await _context.SaveChangesAsync();
            // Redirect to the kitchen view or any other appropriate view
            return RedirectToAction("Bills");
        }
        public async Task<IActionResult> Bills()
        {
            var billOrders = await _context.Order.Include(o=>o.Table).Where(o=>o.status == Models.OrderStatus.BillRequested).OrderBy(o=>o.CreatedAt)
                .ToListAsync();
            return View(billOrders);
        }
        public async Task<IActionResult> BillDetails(int id)
        {
            var order = await _context.Order
         .Include(o => o.Table)
         .Include(o => o.OrderDetails)
           .ThenInclude(oi => oi.FoodItem)
         .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
                return NotFound();
            return View(order);

        }
        public async Task<IActionResult> Dashboard()
        {
            var items = await _context.OrderDetail
            .Include(oi => oi.Order)
        .Include(oi => oi.FoodItem)
        .Where(oi => (oi.Status == OrderDetailStatus.Comfirmed || oi.Status == OrderDetailStatus.Preparing)
          && oi.Order != null && oi.Order.Table != null)
        .OrderBy(oi => oi.Status)
        .ThenBy(oi => oi.OrderId)
        .ToListAsync();
            return View(items);
        }
        public async Task<IActionResult> UpdateStatus(int orderDetailId, int status)
        {
            var item = await _context.OrderDetail.FindAsync(orderDetailId);
            if (item == null)
                return NotFound();
            item.Status = (OrderDetailStatus)status;
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard");
        }


    }
}