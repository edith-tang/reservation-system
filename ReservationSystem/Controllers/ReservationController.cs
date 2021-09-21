using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        #region DI
        private readonly ApplicationDbContext _cxt;
        public ReservationController(ApplicationDbContext cxt)
        {
            _cxt = cxt;
        }
        #endregion

        #region ACTION METHODS
        public async Task<ActionResult> IndexReservation()
        {
            var reservations = await _cxt.Reservations.Include(r => r.Customer).Include(r => r.Sitting).ToListAsync();
            return View(reservations);
        }

        public ActionResult History()
        {
            return View();
        }

        public async Task<ActionResult> DetailsReservation(int id)
        {
            var reservation = await _cxt.Reservations.Include(r => r.Customer).Include(r => r.Sitting).ThenInclude(s => s.SittingCategory)
                .FirstOrDefaultAsync(r => r.Id == id);
            return View(reservation);
        }

        [HttpGet]
        public async Task<ActionResult> CreateReservation()
        {
            var sittings = await GetAllFutureSittings();

            var m = new CreateReservation
            {
                SittingOptions = new List<SittingOption>(),
                SysDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToUpper(),
                MaxDate = sittings.Max(s => s.Date),
            };
            foreach (var s in sittings)
            {
                m.SittingOptions.Add(new SittingOption
                {
                    Id = s.Id,
                    Date = s.Date.ToShortDateString(),
                    SCName = s.SittingCategory.Name,
                    StartTime = s.SittingCategory.StartTime.ToString(@"hh\:mm\:ss"),
                    EndTime = s.SittingCategory.EndTime.ToString(@"hh\:mm\:ss"),
                });
            }
            return View(m);
        }

        [HttpPost]
        public async Task<ActionResult> CreateReservation(CreateReservation m)
        {
            var reservation = new Data.Reservation
            {
                Customer = new Data.Customer
                {
                    CustFName = m.Customer.CustFName,
                    CustLName = m.Customer.CustLName,
                    CustEmail = m.Customer.CustEmail,
                    CustPhone = m.Customer.CustPhone
                },
                SittingId = m.SelectedSittingOptionId,
                NumOfGuests = m.NumOfGuests,
                Notes = m.Notes,
                TimeOfBooking = DateTime.Now,
                Status = Data.Enums.ReservationStatus.Pending,
            };
            _cxt.Reservations.Add(reservation);
            await _cxt.SaveChangesAsync();

            return RedirectToAction(nameof(IndexReservation));
        }
        #endregion

        #region RESERVATION METHODS
        //find all active sittings and dates
        public async Task<List<Sitting>> GetAllFutureSittings()
        {
            return await _cxt.Sittings.Where(s => s.Status != Data.Enums.SittingStatus.Past)
                .Include(s => s.SittingCategory).ThenInclude(s => s.SCTimeslots)
                .ToListAsync();
        }

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

        public async Task<JsonResult> GetTimeslots(int sittingId)
        {
            var sitting = _cxt.Sittings.FirstOrDefault(s => s.Id == sittingId);
            var timeslots = await _cxt.SCTimeslots.Where(s => s.SittingCategoryId == sitting.SittingCategoryId).ToArrayAsync();
            var startTimes = new List<string>();
            foreach(var t in timeslots)
            {
                startTimes.Add(t.StartTime.ToString(@"hh\:mm"));
            }
            return Json(startTimes);
        }
        #endregion
    }
}
