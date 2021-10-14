using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservationSystem.Data;
using ReservationSystem.Models;
using ReservationSystem.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        #region DI
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _cxt;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AdminService _adminService;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext cxt, UserManager<IdentityUser> userManager, AdminService adminService)
        {
            _logger = logger;
            _cxt = cxt;
            _userManager = userManager;
            _adminService = adminService;
        }
        #endregion

        public async Task<IActionResult> Index()
        {

            await _adminService.SeedAdmin();
            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = await _cxt.Users.FirstOrDefaultAsync(u=>u.UserName==User.Identity.Name);

            //    var customer = await _cxt.Customers.FirstOrDefaultAsync(c => c.IdentityUserId == user.Id);

            //    if (!User.IsInRole("Member")) 
            //    { 
            //    await _userManager.AddToRoleAsync(user, "Member");
            //    }
            //}
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult RedirectUser()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index","Home",new { area = "Admin" });
            }
            else if (User.IsInRole("Employee"))
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            
        }
    }
}
