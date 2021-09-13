using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _cxt;

        public ReservationController(ApplicationDbContext cxt)
        {
            _cxt = cxt;
        }
        public IActionResult Index()
        {
            return View();
        }

        //find a sitting from database
        public Sitting FindSitting()
        {
            var sitting = new Sitting();
            return sitting;
        }

        public Customer FindCust()
        {
            var cust = new Customer();
            return cust;
        }

        //multiple filters, consider using interface
        //check reservations for a sitting
        public async Task<List<Reservation>> CheckReservationBySitting()
        {
            var sitting = FindSitting();
            return await _cxt.Reservations.Include(r => r.Sitting).Where(r => r.Sitting == sitting).ToListAsync();
        }

        //check reservations for a customer
        public async Task<List<Reservation>> CheckReservationByCust()
        {
            var cust = FindCust();
            return await _cxt.Reservations.Include(r => r.Customer).Where(r => r.Customer == cust).ToListAsync();
        }
    }
}
