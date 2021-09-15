using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models.Reservation;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            var sittings = await GetAllFutureSittings();
            var m = new Models.Reservation.CreateReservation
            {
                SysDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToUpper(),
                MaxDate = sittings.Max(s => s.Date),
                AvailableSittings = new List<AvailableSitting>()
            };
            foreach (var s in sittings)
            {
                m.AvailableSittings.Add(new AvailableSitting { Id = s.Id, Date = s.Date.ToShortDateString() });
            }
            return View(m);
        }

        #region RESERVATION METHODS
        //find all active sittings and dates
        public async Task<List<Sitting>> GetAllFutureSittings()
        {
            return await _cxt.Sittings.Where(s => s.Status != Data.Enums.SittingStatus.Past).OrderBy(s => s.Date).ToListAsync();
        }

        public Customer FindCustomer()
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
            var cust = FindCustomer();
            return await _cxt.Reservations.Include(r => r.Customer).Where(r => r.Customer == cust).ToListAsync();
        }
        #endregion
    }
}
