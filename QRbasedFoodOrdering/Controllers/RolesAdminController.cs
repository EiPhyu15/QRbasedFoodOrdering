using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using QRbasedFoodOrdering.Models;
using QRbasedFoodOrdering.Data;
using Microsoft.AspNetCore.Authorization;


namespace QRbasedFoodOrdering.Controllers
{
   // [Authorize(Roles ="SuperAdmin")]
    public class RolesAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesAdminController(ApplicationDbContext context,
UserManager<IdentityUser> userManager, RoleManager<IdentityRole>
roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var roles = _context.Roles.OrderBy(r => r.Name).ToList();
            //select Name from Roles Order by Names;
            return View(roles);
        }
        public IActionResult ListUsers()
        {
            var users = _context.Users.OrderBy(u => u.UserName).ToList();
            //Select UserName from Users Order By UserName
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            IdentityResult ir = new IdentityResult();
            var _role = new IdentityRole();
            _role.Name = collection["RoleName"];
            ir = await _roleManager.CreateAsync(_role);
            if (!ir.Succeeded)
            {
                foreach (IdentityError er in ir.Errors)
                {
                    ModelState.AddModelError(string.Empty,
                   er.Description);
                }
                ViewBag.UserMessage = "Error Addding Role";
                return View();
            }
            else
            {
                ViewBag.UserMessage = "Role Added";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(IFormCollection form)
        {
            UserRoleViewModel vm = new UserRoleViewModel();
            var username = form["UserName"];
            var user = _context.Users.Where(u => u.UserName == username.ToString()).FirstOrDefault();
            if (user != null)
            {
                var roles = _context.Roles.ToList();
                vm.UserName = username;
                vm.UserId = user.Id;
                foreach (var item in roles)
                {
                    RoleAssignment roleAssigned = new RoleAssignment();
                    roleAssigned.Name = item.Name;
                    roleAssigned.Id = item.Id;

                    if (await _userManager.IsInRoleAsync(user, item.Name))
                    {
                        roleAssigned.IsChecked = true;
                    }
                    else
                    {
                        roleAssigned.IsChecked = false;
                    }
                    vm.UserRoles.Add(roleAssigned);
                }
            }
            else
            {
                vm = null;
            }

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRoles(UserRoleViewModel updateRoles, IFormCollection form)
        {
            var user = _context.Users.Where(u => u.UserName ==
           form["UserName"].ToString()).FirstOrDefault();
            foreach (var item in updateRoles.UserRoles)
            {
                if (item.IsChecked)
                {
                    if (!await _userManager.IsInRoleAsync(user, item.Name))
                    {
                        await _userManager.AddToRoleAsync(user, item.Name);

                    }
                }
                else if (await _userManager.IsInRoleAsync(user, item.Name))
                {
                    await _userManager.RemoveFromRoleAsync(user,
                   item.Name);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
