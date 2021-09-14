using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [HttpGet]
        public async Task<ActionResult> CreateReservation()
        {
            var allSittings = await GetAllFutureSittings();
            var dates = new List<DateTime>();
            foreach(var s in allSittings)
            {
                dates.Add(s.Date);
            }
            var m = new Models.Reservation.CreateReservation
            {
                AllSittings = new SelectList(allSittings),
                Dates = new SelectList(dates), 
            };
            return View(m);
        }

        //find all active sittings and dates
        public async Task<List<Sitting>> GetAllFutureSittings()
        {
            return await _cxt.Sittings.Where(s => s.Status != Data.Enums.SittingStatus.Past).OrderBy(s => s.Date).ToListAsync();
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
            var sitting = new Sitting();
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
