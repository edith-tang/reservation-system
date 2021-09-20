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

        public async Task<ActionResult> Index()
        {
            var reservations = await _cxt.Reservations.Include(r => r.Customer).Include(r => r.Sitting).ToListAsync();
            return View(reservations);
        }

        public ActionResult History()
        {
            return View();
        }

        public async Task<ActionResult> Details(int id)
        {
            var reservation = await _cxt.Reservations.Include(r => r.Customer).Include(r => r.Sitting).ThenInclude(s => s.SittingCategory)
                .FirstOrDefaultAsync(r => r.Id == id);
            return View(reservation);
        }

            [HttpGet]
        public async Task<ActionResult> CreateReservation()
        {
            var sittings = await GetAllFutureSittings();
            var m = new Models.Reservation.CreateReservation
            {
                SysDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToUpper(),
                MaxDate = sittings.Max(s => s.Date),
                FutureSittings = new List<FutureSitting>()
            };
            foreach (var s in sittings)
            {
                m.FutureSittings.Add(new FutureSitting
                {
                    Id = s.Id,
                    Date = s.Date.ToShortDateString(),
                    SCName = s.SittingCategory.Name,
                    StartTime = s.SittingCategory.StartTime.ToString(@"hh\:mm\:ss"),
                    EndTime = s.SittingCategory.EndTime.ToString(@"hh\:mm\:ss"),
                }); ;
            }
            return View(m);
        }

        [HttpPost]
        public async Task<ActionResult> CreateReservation(CreateReservation m)
        {
            var reservation = new Data.Reservation
            {
                CustomerId = 1,
                SittingId = m.SelectedSittingId,
                NumOfGuests = m.NumOfGuests,
                StartTime = m.StartTime,
                Duration = m.Duration,
                Notes = m.Notes,
                TimeOfBooking = DateTime.Now,
                Status = Data.Enums.ReservationStatus.Pending,
            };
            _cxt.Reservations.Add(reservation);
            await _cxt.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        #region RESERVATION METHODS
        //find all active sittings and dates
        public async Task<List<Sitting>> GetAllFutureSittings()
        {
            return await _cxt.Sittings.Where(s => s.Status != Data.Enums.SittingStatus.Past).Include(s => s.SittingCategory).ToListAsync();
        }

        //multiple filters, consider using interface
        //check reservations for a sitting
        public async Task<List<Reservation>> CheckReservationBySitting()
        {
            var sitting = new Sitting();
            return await _cxt.Reservations.Include(r => r.Sitting).Where(r => r.Sitting == sitting).ToListAsync();
        }

        //check reservations for a customer
        public async Task<List<Reservation>> CheckReservationByCust(int customerId)
        {
            return await _cxt.Reservations.Include(r => r.Customer).Where(r => r.Id == customerId).ToListAsync();
        }
        #endregion
    }
}
