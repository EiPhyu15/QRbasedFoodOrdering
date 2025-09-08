using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QRbasedFoodOrdering.Data;
using QRbasedFoodOrdering.Models;
using QRCoder;

namespace QRbasedFoodOrdering.Controllers
{
    
    public class TablesController : Controller
    {
        private readonly ApplicationDbContext _context;
       

        public TablesController(ApplicationDbContext context)
        {
            _context = context;
            
        }
        //
        public async Task<IActionResult> Assign1()
        {
            var tables = await _context.Table.ToListAsync(); // load ALL tables
            return View(tables);
        }

        [HttpPost]
        //public async Task<IActionResult> FreeTable(int tableId)
        //{
        //    var table = await _context.Table.FindAsync(tableId);
        //    if (table == null) return NotFound();

        //    table.Status = TableStatus.Available;
        //    _context.Update(table);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(Assign1));
        //}
       
        public async Task<IActionResult> FreeTable(int tableId)
        {
            var table = await _context.Table
                .Include(t => t.Orders) // ✅ load related orders
                .FirstOrDefaultAsync(t => t.TableId == tableId);

            if (table == null) return NotFound();

            // double-check before freeing
            if (table.Orders.Any(o => o.status != OrderStatus.Completed))
            {
                TempData["Error"] = "You cannot free this table until all orders are completed.";
                return RedirectToAction(nameof(Assign1));
            }

            table.Status = TableStatus.Available;
            _context.Update(table);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Table has been freed.";
            return RedirectToAction(nameof(Assign1));
        }




        //public async Task<IActionResult> Assign()
        //{
        //    var availableTables = await _context.Table.Where(t=>t.Status == TableStatus.Available)
        //        .Include(t=>t.Orders).ToListAsync();
        //    return View(availableTables);

        //}



        public async Task<IActionResult> AssignTable(int tableId)
        {
            var table = await _context.Table.FindAsync(tableId);
            if (table == null)
            {
                return NotFound();
            }

            //var openorder = await _context.Order
            //     .FirstOrDefaultAsync(o => o.TableId == tableId && o.status == OrderStatus.Pending || o.status == OrderStatus.
            //if (openorder != null)
            //{
            //    // If an open order exists, redirect to the order details page
            //    return BadRequest("An open order already exists for this table.");
            //}
            var guid = Guid.NewGuid().ToString();
            var order = new Order
            {
                TableId = tableId,
                CreatedAt = DateTime.Now,
                QRCode = guid,
                status = OrderStatus.Pending,
            };
            _context.Order.Add(order);

            await _context.SaveChangesAsync();

            if (table != null)
            {
                table.Status = TableStatus.Occupied; // Mark the table as occupied
                _context.Update(table);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("QRCode", new { orderId = order.OrderId });

        }
        public async Task<IActionResult> QRCode(int orderId)
        {
            var order = await _context.Order
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound();
            }


            ViewBag.Order = order;
            return View(order);


        }
        public IActionResult GenerateQRCodeImage(string guid)
        {
            var url = Url.Action("Menu", "Order", new { guid = guid }, Request.Scheme, Request.Host.ToString());
            if (string.IsNullOrEmpty(url))
                return BadRequest("Unable to generate QR code URL.");
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var pngQrCode = new PngByteQRCode(qrData);
            var pngBytes = pngQrCode.GetGraphic(20);
            return File(pngBytes, "image/png");
        }


        // GET: Tables
        public async Task<IActionResult> Index()
        {
            return View(await _context.Table.ToListAsync());
        }

        // GET: Tables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Table
                .FirstOrDefaultAsync(m => m.TableId == id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // GET: Tables/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TableId,TableName,Capacity,IsAvailable")] Table table)
        {
           // if (ModelState.IsValid)
            //{
                _context.Add(table);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Assign1));
            //}
            return View(table);
        }

        // GET: Tables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Table.FindAsync(id);
            if (table == null)
            {
                return NotFound();
            }
            return View(table);
        }

        // POST: Tables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TableId,TableName,Capacity,IsAvailable")] Table table)
        {
            if (id != table.TableId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(table);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableExists(table.TableId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            //}
            return View(table);
        }

        // GET: Tables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Table
                .FirstOrDefaultAsync(m => m.TableId == id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // POST: Tables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var table = await _context.Table.FindAsync(id);
            if (table != null)
            {
                _context.Table.Remove(table);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TableExists(int id)
        {
            return _context.Table.Any(e => e.TableId == id);
        }
    }
}
