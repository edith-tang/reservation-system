using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public abstract class AdminAreaBaseControlller : Controller
    {

        protected readonly ApplicationDbContext _cxt;

        public AdminAreaBaseControlller(ApplicationDbContext cxt)
        {
            _cxt = cxt;
        }
        
    }
}
