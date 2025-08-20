using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Identity.Client;
using QRbasedFoodOrdering.Data;
using QRbasedFoodOrdering.Models;

namespace QRbasedFoodOrdering.Controllers
{
    public class CartItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public string ItemCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public CartItemsController(ApplicationDbContext context)
        {
            this.ItemCartId = "";
            _context = context;

        }
        public async Task<IActionResult> AddToCart(int foodid, int tableid)
        {
            ItemCartId = GetCartId();
            var cartItem = await _context.CartItem
                    .FirstOrDefaultAsync(c => c.FoodItemId == foodid && c.TableId == tableid);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    FoodItemId = foodid,
                    TableId = tableid,
                    Quantity = 1,
                    Price = (int)_context.FoodItem.FirstOrDefault(f => f.FoodItemId == foodid).Price
                };
                _context.CartItem.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
                //cartItem.Price += _context.FoodItem.FirstOrDefault(f => f.FoodItemId == foodid).Price;
                //_context.CartItem.Update(cartItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("DisplayCart", new {id=tableid});
        }
        public string GetCartId()
        {
            var session = HttpContext.Session.GetString(CartSessionKey);
            if (session == null)
            {
                if (!string.IsNullOrWhiteSpace(User.Identity.Name))
                {
                    session = User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class.
                    Guid tempCartId = Guid.NewGuid();
                    session = tempCartId.ToString();
                }
            }
            return session.ToString();
        }
       public List<CartItem> GetCartItems()
        {
            ItemCartId = GetCartId();
           return _context.CartItem
                .Where(c => c.CartId == ItemCartId)
                
                .ToList();
        }
        public async Task<IActionResult> DisplayCart(int id)
        {
            var cartitems= GetCartItems();
            ViewBag.count = cartitems.Count;
            return View(cartitems);
        }
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var cartItemDelete = _context.CartItem.Find(id);
            if(cartItemDelete != null)
            {
                _context.CartItem.Remove(cartItemDelete);
               
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("DisplayCart");
        }
     
      

        // GET: CartItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CartItem.Include(c => c.FoodItem).Include(c => c.Table);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CartItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .Include(c => c.FoodItem)
                .Include(c => c.Table)
                .FirstOrDefaultAsync(m => m.CartItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: CartItems/Create
        public IActionResult Create()
        {
            ViewData["FoodItemId"] = new SelectList(_context.FoodItem, "FoodItemId", "FoodItemId");
            ViewData["TableId"] = new SelectList(_context.Table, "TableId", "TableId");
            return View();
        }

        // POST: CartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartItemId,Quantity,Price,FoodItemId,TableId")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FoodItemId"] = new SelectList(_context.FoodItem, "FoodItemId", "FoodItemId", cartItem.FoodItemId);
            ViewData["TableId"] = new SelectList(_context.Table, "TableId", "TableId", cartItem.TableId);
            return View(cartItem);
        }

        // GET: CartItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            ViewData["FoodItemId"] = new SelectList(_context.FoodItem, "FoodItemId", "FoodItemId", cartItem.FoodItemId);
            ViewData["TableId"] = new SelectList(_context.Table, "TableId", "TableId", cartItem.TableId);
            return View(cartItem);
        }

        // POST: CartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartItemId,Quantity,Price,FoodItemId,TableId")] CartItem cartItem)
        {
            if (id != cartItem.CartItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemExists(cartItem.CartItemId))
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
            ViewData["FoodItemId"] = new SelectList(_context.FoodItem, "FoodItemId", "FoodItemId", cartItem.FoodItemId);
            ViewData["TableId"] = new SelectList(_context.Table, "TableId", "TableId", cartItem.TableId);
            return View(cartItem);
        }

        // GET: CartItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .Include(c => c.FoodItem)
                .Include(c => c.Table)
                .FirstOrDefaultAsync(m => m.CartItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // POST: CartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartItem = await _context.CartItem.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItem.Remove(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItem.Any(e => e.CartItemId == id);
        }
    }
}
