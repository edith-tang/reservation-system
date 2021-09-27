using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Controllers
{
    public class HomeController : AdminAreaBaseControlller
    {
        public HomeController(ApplicationDbContext cxt) : base(cxt)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
