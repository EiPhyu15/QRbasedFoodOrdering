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
    public class OrderDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OrderDetail.Include(o => o.FoodItem).Include(o => o.Order);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetail
                .Include(o => o.FoodItem)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }
        // GET: OrderDetails/Create
        //public IActionResult Create(int? foodItemId)
        //{
        //    var model = new OrderDetail();

        //    if (foodItemId.HasValue)
        //    {
        //        model.FoodItemId = foodItemId.Value;

        //        // get food details
        //        var food = _context.FoodItem.Find(foodItemId.Value);
        //        if (food != null)
        //        {
        //            model.Price = food.Price;
        //            ViewBag.FoodName = food.Title;
        //            ViewBag.FoodPreselected = true; // ✅ flag for view
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.FoodPreselected = false; // normal dropdown
        //    }

        //    ViewBag.OrderId = new SelectList(_context.Order, "OrderId", "OrderNumber");
        //    ViewBag.FoodId = new SelectList(_context.FoodItem, "FoodItemId", "FoodName", model.FoodItemId);

        //    ViewBag.Status = Enum.GetValues(typeof(OrderStatus))
        //                         .Cast<OrderStatus>()
        //                         .Select(s => new SelectListItem
        //                         {
        //                             Value = s.ToString(),
        //                             Text = s.ToString()
        //                         }).ToList();

        //    return View(model);
        //}


        // GET: OrderDetails/Create
        public IActionResult Create()
        {
            ViewData["FoodItemId"] = new SelectList(_context.FoodItem, "FoodItemId", "FoodItemId");
            ViewData["OrderId"] = new SelectList(_context.Order, "OrderId", "OrderId");
            ViewBag.OrderId = new SelectList(_context.Order, "OrderId", "OrderNumber");
            ViewBag.FoodId = new SelectList(_context.FoodItem, "FoodItemId", "FoodName");

            ViewBag.StatusList = new SelectList(Enum.GetValues(typeof(OrderDetailStatus)));
            ViewBag.OrderId = new SelectList(_context.Order, "OrderId", "OrderNumber");

            // Foods dropdown
            ViewBag.FoodId = new SelectList(_context.FoodItem, "FoodItemId", "FoodName");

            // Enum dropdown for Status
            ViewBag.Status = Enum.GetValues(typeof(OrderStatus))
                                 .Cast<OrderStatus>()
                                 .Select(s => new SelectListItem
                                 {
                                     Value = s.ToString(),
                                     Text = s.ToString()
                                 }).ToList();
            return View();
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]


        //public async Task<IActionResult> Create([Bind("OrderDetailId,Quantity,Status,OrderId,FoodItemId")] OrderDetail orderDetail)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // ✅ Fetch correct price from DB
        //        var food = await _context.FoodItem.FindAsync(orderDetail.FoodItemId);
        //        if (food != null)
        //        {
        //            orderDetail.Price = food.Price;
        //        }

        //        // ✅ Calculate total
        //        orderDetail.Price= orderDetail.Quantity * orderDetail.Price;

        //        _context.Add(orderDetail);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewBag.OrderId = new SelectList(_context.Order, "OrderId", "OrderNumber", orderDetail.OrderId);
        //    ViewBag.FoodId = new SelectList(_context.FoodItem, "FoodItemId", "FoodName", orderDetail.FoodItemId);
        //    ViewBag.Status = Enum.GetValues(typeof(OrderStatus))
        //                         .Cast<OrderStatus>()
        //                         .Select(s => new SelectListItem
        //                         {
        //                             Value = s.ToString(),
        //                             Text = s.ToString()
        //                         }).ToList();

        //    return View(orderDetail);
        //}

        public async Task<IActionResult> Create([Bind("OrderDetailId,Quantity,Price,Status,OrderId,FoodItemId")] OrderDetail orderDetail)
        {
            //if (ModelState.IsValid)
            //{
            _context.Add(orderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //}
            //ViewData["FoodItemId"] = new SelectList(_context.FoodItem, "FoodItemId", "FoodItemId", orderDetail.FoodItemId);
            // ViewData["OrderId"] = new SelectList(_context.Order, "OrderId", "OrderId", orderDetail.OrderId);
            ViewBag.OrderId = new SelectList(_context.Order, "OrderId", "OrderNumber", orderDetail.OrderId);
            ViewBag.FoodId = new SelectList(_context.FoodItem, "FoodItemId", "FoodName", orderDetail.FoodItemId);
            ViewBag.Status = Enum.GetValues(typeof(OrderStatus))
                                 .Cast<OrderStatus>()
                                 .Select(s => new SelectListItem
                                 {
                                     Value = s.ToString(),
                                     Text = s.ToString()
                                 }).ToList();
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetail.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["FoodItemId"] = new SelectList(_context.FoodItem, "FoodItemId", "FoodItemId", orderDetail.FoodItemId);
            ViewData["OrderId"] = new SelectList(_context.Order, "OrderId", "OrderId", orderDetail.OrderId);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderDetailId,Quantity,Price,Status,OrderId,FoodItemId")] OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.OrderDetailId))
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
            ViewData["FoodItemId"] = new SelectList(_context.FoodItem, "FoodItemId", "FoodItemId", orderDetail.FoodItemId);
            ViewData["OrderId"] = new SelectList(_context.Order, "OrderId", "OrderId", orderDetail.OrderId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetail
                .Include(o => o.FoodItem)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetail.FindAsync(id);
            if (orderDetail != null)
            {
                _context.OrderDetail.Remove(orderDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetail.Any(e => e.OrderDetailId == id);
        }
    }
}
