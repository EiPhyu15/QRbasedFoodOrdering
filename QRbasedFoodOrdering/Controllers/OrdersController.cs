using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QRbasedFoodOrdering.Data;
using QRbasedFoodOrdering.Models;

namespace QRbasedFoodOrdering.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Menu(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return NotFound();
            var order = await _context.Order.FirstOrDefaultAsync(o => o.QRCode == guid && (o.status == OrderStatus.Pending || o.status == OrderStatus.Comfirmed));
            if (order == null)
                return View("OrderNotFound");
            var categories = await _context.Category.Include(c => c.FoodItems).ToListAsync();
            ViewBag.OrderId = order.OrderId;
            ViewBag.Guid = guid;
            return View(categories);

        }
        public async Task<IActionResult> AddToCart(int quantity, int fooditemid, int orderId)
       {
           if ( quantity <1)
            {
                return BadRequest();
            }
            var order = await _context.Order.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            var foodItem = await _context.FoodItem.FindAsync(fooditemid);
            if (foodItem == null || !foodItem.IsActive)
            {
                return NotFound();
            }
            var existingOrderDetail = await _context.OrderDetail
                .FirstOrDefaultAsync(od => od.OrderId == order.OrderId && od.FoodItemId == fooditemid);
            if (existingOrderDetail != null)
            {
                existingOrderDetail.Quantity += quantity;

            }
            else
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    FoodItemId = fooditemid,
                    Quantity = quantity,
                    Price = foodItem.Price,
                    Status = OrderDetailStatus.Pending
                };
                _context.OrderDetail.Add(orderDetail);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Menu", new { guid = order.QRCode });
        }
        public async Task<IActionResult> Cart(string guid)
        {
            if(string.IsNullOrEmpty(guid))
                return NotFound();
            var order = await _context.Order.Include(o => o.Table)
                
                .FirstOrDefaultAsync(o => o.QRCode == guid && (o.status == OrderStatus.Pending || o.status == OrderStatus.Comfirmed || o.status==OrderStatus.BillRequested));
            if (order == null)
            {
                return NotFound();
            }
            var pendingitem= await _context.OrderDetail
                .Include(od => od.FoodItem)
                .Where(od => od.OrderId == order.OrderId && od.Status == OrderDetailStatus.Pending)
                .ToListAsync();
            var ComfirmedItems = await _context.OrderDetail
                .Include(od => od.FoodItem)
                .Where(od => od.OrderId == order.OrderId && od.Status == OrderDetailStatus.Comfirmed)
                .ToListAsync();
            ViewBag.Order = order;
            //ViewBag.PendingItems = pendingitem;
            ViewBag.ComfirmedItems = ComfirmedItems;
            return View(pendingitem);
        }
        public async Task<IActionResult> ConfirmOrder(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return NotFound();
            var order = await _context.Order.FirstOrDefaultAsync(o => o.QRCode == guid && (o.status == OrderStatus.Pending || o.status == OrderStatus.Comfirmed));
            if (order == null)
            {
                return NotFound();
            }
            var pendingItems = await _context.OrderDetail
                .Where(od => od.OrderId == order.OrderId && od.Status == OrderDetailStatus.Pending)
                .ToListAsync();
            if (!pendingItems.Any())
            {
                TempData["Message"] = "There are no pending items to confirm.";
                return RedirectToAction("Cart", new { guid });
            }
            foreach (var item in pendingItems)
            {
                item.Status = OrderDetailStatus.Comfirmed;
            }
            order.status = OrderStatus.Comfirmed;
            await _context.SaveChangesAsync();
            TempData["Message"] = "Order confirmed successfully.";
            return RedirectToAction("Cart", new { guid });



        }
        public async Task<IActionResult> RequestBill(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return NotFound();
            var order = await _context.Order.FirstOrDefaultAsync(o => o.QRCode == guid && (o.status == OrderStatus.Comfirmed || o.status == OrderStatus.BillRequested));
            if (order == null)
            {
                return NotFound();
            }


            order.status = OrderStatus.BillRequested;
            await _context.SaveChangesAsync();
            TempData["BillRequested"] = true;

            return RedirectToAction("Cart", new { guid });

        }
        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Order.Include(o => o.Table);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Table)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["TableId"] = new SelectList(_context.Table, "TableId", "TableId");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,TableId,CreatedAt,status")] Order order)
        {
            //if (ModelState.IsValid)
           //{
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
           // }
            ViewData["TableId"] = new SelectList(_context.Table, "TableId", "TableId", order.TableId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["TableId"] = new SelectList(_context.Table, "TableId", "TableId", order.TableId);
            return View(order);
        }
       
        


        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,TableId,CreatedAt,status")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TableId"] = new SelectList(_context.Table, "TableId", "TableId", order.TableId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Table)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }
    }
}
