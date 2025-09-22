using Microsoft.AspNetCore.Authorization;
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
        //[Authorize(Roles = "Admin")]


        [HttpPost]
        public async Task<IActionResult> MarkPaid(int orderId)
        {
            var order = await _context.Order.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            // Update the order status to Paid
            order.status = OrderStatus.Completed;
            _context.Order.Update(order);
            await _context.SaveChangesAsync();
            // Redirect to the kitchen view or any other appropriate view
            return RedirectToAction("BillsS");
            //return View();
        }
        public async Task<IActionResult> Bills()
        {
            var billOrders = await _context.Order.Include(o=>o.Table).Where(o=>o.status == OrderStatus.BillRequested).OrderBy(o=>o.CreatedAt)
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
                    .ThenInclude(o => o.Table)  
                .Include(oi => oi.FoodItem)
                .Where(oi => (oi.Status == OrderDetailStatus.Comfirmed
                           || oi.Status == OrderDetailStatus.Preparing)
                           && oi.Order != null && oi.Order.Table != null)
                .OrderBy(oi => oi.Status)
                .ThenBy(oi => oi.OrderId)
                .ToListAsync();

            return View(items);
        }

        //public async Task<IActionResult> UpdateStatus(int orderDetailId, int status)
        //{
        //    var item = await _context.OrderDetail.FindAsync(orderDetailId);
        //    if (item == null)
        //        return NotFound();
        //    item.Status = (OrderDetailStatus)status;
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("Dashboard");
        //}

        public async Task<IActionResult> UpdateStatus(int orderDetailId, int status)
        {
            var item = await _context.OrderDetail
                .Include(od => od.Order)
                .ThenInclude(o => o.Table)
                .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);

            if (item == null)
                return NotFound();

            item.Status = (OrderDetailStatus)status;

            if (item.Order?.Table != null)
            {
                // If first item starts preparing → set table Preparing
                if (status == (int)OrderDetailStatus.Preparing)
                {
                    item.Order.Table.Status = TableStatus.Preparing;
                }

                // If all items served → free the table
                if (status == (int)OrderDetailStatus.Served)
                {
                    var allServed = await _context.OrderDetail
                        .Where(od => od.OrderId == item.OrderId)
                        .AllAsync(od => od.Status == OrderDetailStatus.Served);

                    if (allServed)
                    {
                        item.Order.Table.Status = TableStatus.Available;
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }
        //public async Task<IActionResult> UpdateStatus(int orderDetailId, int status)
        //{
        //    var item = await _context.OrderDetail
        //        .Include(od => od.Order)
        //        .ThenInclude(o => o.Table)
        //        .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);

        //    if (item == null)
        //        return NotFound();

        //    item.Status = (OrderDetailStatus)status;
        //    await _context.SaveChangesAsync(); // save first so DB is up to date

        //    if (item.Order?.Table != null)
        //    {
        //        // If first item starts preparing → set table Preparing
        //        if (status == (int)OrderDetailStatus.Preparing)
        //        {
        //            item.Order.Table.Status = TableStatus.Preparing;
        //        }

        //        // If all items served → free the table
        //        else if (status == (int)OrderDetailStatus.Served)
        //        {
        //            var allServed = await _context.OrderDetail
        //                .Where(od => od.OrderId == item.OrderId)
        //                .AllAsync(od => od.Status == OrderDetailStatus.Served);

        //            if (allServed)
        //            {
        //                item.Order.Table.Status = TableStatus.Available;
        //            }
        //        }
        //        _context.Update(item.Order.Table);
        //        await _context.SaveChangesAsync();
        //    }

        //    // save table status change
        //    return RedirectToAction("Dashboard");
        //}
        //public async Task<IActionResult> UpdateStatus(int orderDetailId, int status)
        //{
        //    var item = await _context.OrderDetail
        //        .Include(od => od.Order)
        //        .ThenInclude(o => o.Table)
        //        .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);

        //    if (item == null)
        //        return NotFound();

        //    // Update item status
        //    item.Status = (OrderDetailStatus)status;
        //    await _context.SaveChangesAsync(); // Save item first

        //    if (item.Order?.Table != null)
        //    {
        //        if (status == (int)OrderDetailStatus.Preparing)
        //        {
        //            item.Order.Table.Status = TableStatus.Preparing;
        //            item.Order.status = OrderStatus.Preparing;
        //        }
        //        else if (status == (int)OrderDetailStatus.Served)
        //        {
        //            // Check if ALL items in the order are served
        //            var allServed = await _context.OrderDetail
        //                .AsNoTracking()
        //                .Where(od => od.OrderId == item.OrderId)
        //                .AllAsync(od => od.Status == OrderDetailStatus.Served);

        //            if (allServed)
        //            {
        //                // Mark order completed
        //                item.Order.status = OrderStatus.Completed;

        //                // Free table automatically
        //                item.Order.Table.Status = TableStatus.Available;
        //            }
        //        }

        //        _context.Update(item.Order);
        //        _context.Update(item.Order.Table);
        //        await _context.SaveChangesAsync();
        //    }

        //    return RedirectToAction("Dashboard");
        //}

    }
}