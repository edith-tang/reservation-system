using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReservationSystem.Data;
using ReservationSystem.Areas.Admin.Models.Reservation;
using ReservationSystem.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Controllers
{
    public class ReservationController : AdminAreaBaseController
    {
        #region DI
        private readonly CustomerService _customerService;
        public ReservationController(ApplicationDbContext cxt, UserManager<IdentityUser> userManager, CustomerService customerService) : base(cxt, userManager)
        {
            _customerService = customerService;
        }
        #endregion

        #region ACTION METHODS
        public async Task<ActionResult> IndexReservation()
        {
            var reservations = await _cxt.Reservations.Include(r => r.Customer).Include(r => r.Sitting).OrderBy(r => r.TimeOfBooking).ToListAsync();
            return View(reservations);
        }

        //check a customer's history
        public async Task<ActionResult> HistoryReservation(int id)
        {
            var reservation = await _cxt.Reservations
                .Include(r => r.Sitting)
                .Include(r => r.Customer).Where(r => r.CustomerId == id)
                .OrderBy(r => r.Sitting.Date)
                .ToListAsync();
            return View(reservation);
        }

        //check a customer's reservation detail
        public async Task<ActionResult> DetailsReservation(int id)
        {
            var reservation = await _cxt.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Sitting.SittingCategory)
                .FirstOrDefaultAsync(r => r.Id == id);
            return View(reservation);
        }

        //admin and employee creates reservation for customer
        [HttpGet]
        public async Task<ActionResult> CreateReservation()
        {
            var sittings = await GetAllFutureSittings();
            var m = new CreateReservation
            {
                MaxDate = sittings.Max(s => s.Date).ToString("yyyy-MM-dd"),
                MinDate = DateTime.Today.ToString("yyyy-MM-dd"),
                Customer = new CustomerDTO(),
                WayOfBookings = new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Email", Value = "Email"},
                        new SelectListItem { Text = "Phone", Value ="Phone"},
                        new SelectListItem { Text = "Walk-in", Value ="Walk-in"},
                    }, "Value", "Text"),
            };
            return View(m);
        }

        //admin and employee submits reservation for customer
        [HttpPost]
        public async Task<ActionResult> CreateReservation(CreateReservation m)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await DataValidation(m);

                    var reservation = new Data.Reservation
                    {
                        SittingId = m.SelectedSittingId,
                        ExpectedStartTime = TimeSpan.Parse(m.ExpectedStartTime),
                        ExpectedEndTime = TimeSpan.Parse(m.ExpectedEndTime),
                        NumOfGuests = m.NumOfGuests,
                        Notes = m.Notes,
                        TimeOfBooking = DateTime.Now,
                        WayOfBooking = m.WayOfBooking,
                        Status = Data.Enums.ReservationStatus.Pending,
                    };

                    await MembershipAndEmailValidation(m, reservation);

                    _cxt.Reservations.Add(reservation);
                    await _cxt.SaveChangesAsync();

                    var sitting = _cxt.Sittings.Include(s=>s.Reservations).FirstOrDefault(s => s.Id == reservation.SittingId);
                    if (sitting.RemainingCapacity <= 0) { sitting.Status = Data.Enums.SittingStatus.Closed; }

                    await _cxt.SaveChangesAsync();

                    //for employees: redirect to all reservations page
                    return RedirectToAction(nameof(IndexReservation));
                }
                catch (Exception)
                {
                    //input inconsistent with database
                    //ModelState.AddModelError("Error", "SQL Error!");
                }
            }
            m.WayOfBookings = new SelectList(
                   new List<SelectListItem>
                   {
                        new SelectListItem { Text = "Email", Value = "Email"},
                        new SelectListItem { Text = "Phone", Value ="Phone"},
                        new SelectListItem { Text = "Walk-in", Value ="Walk-in"},
                   }, "Value", "Text");
            return View(m);
        }

        public async Task<IActionResult> CancelReservation(int id)
        {
            var r = _cxt.Reservations.Include(r => r.Sitting).FirstOrDefault(r => r.Id == id);
            var su = _cxt.SittingUnits.Where(su => su.ReservationId == id).ToList();
            r.Status = Data.Enums.ReservationStatus.Cancelled;
            if (r.Sitting.RemainingCapacity > 0) { r.Sitting.Status = Data.Enums.SittingStatus.Open; }
            su.ForEach(su => { su.ReservationId = null; su.Status = Data.Enums.SittingUnitStatus.Available; });
            await _cxt.SaveChangesAsync();
            return RedirectToAction(nameof(IndexReservation));
        }

        #endregion

        #region Methods

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

        //find all active sittings and dates
        public async Task<List<Sitting>> GetAllFutureSittings()
        {
            return await _cxt.Sittings.Where(s => s.Status == Data.Enums.SittingStatus.Open)
                .Include(s => s.SittingCategory).ThenInclude(s => s.SCTimeslots)
                .ToListAsync();
        }

        #endregion

        #region Methods for Ajax Requests
        //find sittings for a specific date
        public async Task<JsonResult> GetSittingsByDate(string date)
        {
            var sittings = await GetAllFutureSittings();
            var filteredSittings = sittings.FindAll(s => s.Date == DateTime.Parse(date));
            var sittingOptions = new List<SittingDTO>();
            foreach (var i in filteredSittings)
            {
                sittingOptions.Add(new SittingDTO
                {
                    Id = i.Id,
                    Date = i.Date.ToShortDateString(),
                    SCName = i.SittingCategory.Name,
                    StartTime = i.SittingCategory.StartTime.ToString(@"hh\:mm\:ss"),
                    EndTime = i.SittingCategory.EndTime.ToString(@"hh\:mm\:ss"),
                });
            }
            return Json(sittingOptions);
        }

        //find a sititng by id
        public async Task<Sitting> GetSittingById(int sittingId)
        {
            return await _cxt.Sittings
                .Include(s => s.Reservations)
                .Include(s => s.SittingCategory).ThenInclude(s => s.SCTimeslots)
                .FirstOrDefaultAsync(s => s.Id == sittingId);
        }

        //find the remaining capacity for a sitting
        public async Task<int> GetSittingCapacity(int sittingId)
        {
            var sitting = await GetSittingById(sittingId);
            return sitting.RemainingCapacity;
        }

        //find all starttimes for a sitting
        public async Task<JsonResult> GetStartTimes(int sittingId)
        {
            var sitting = await GetSittingById(sittingId);
            var startTimes = sitting.SittingStartTimes;
            var startTimesJson = new List<string>();
            foreach (var s in startTimes)
            {
                startTimesJson.Add(s.ToString(@"hh\:mm"));
            }
            return Json(startTimesJson);
        }

        //find all possible endtimes for a starttime of a sitting
        public async Task<JsonResult> GetEndTimes(int sittingId, string startTime)
        {
            var sitting = await GetSittingById(sittingId);
            var endTimes = sitting.SittingEndTimes;
            var endTimesJson = new List<string>();
            var st = TimeSpan.Parse(startTime);
            foreach (var e in endTimes)
            {
                if (st < e) { endTimesJson.Add(e.ToString(@"hh\:mm")); }
            }
            return Json(endTimesJson);
        }

        #endregion

        #region Methods for [HttpPost] CreateReservation()
        public async Task DataValidation(CreateReservation m)
        {
            var sitting = await GetSittingById(m.SelectedSittingId);
            bool checkDate = sitting.Date == m.SelectedDate;
            bool checkStartTime = CheckStartTime(sitting, m.ExpectedStartTime);
            bool checkEndTime = CheckEndTime(sitting, m.ExpectedEndTime);
            bool checkCapacity = 0 < m.NumOfGuests && m.NumOfGuests <= sitting.RemainingCapacity;
            if (!(checkDate && checkStartTime && checkEndTime && checkCapacity))
            {
                throw new Exception();
            }
        }

        public bool CheckStartTime(Sitting sitting, string expectedStartTime)
        {
            var startTimes = sitting.SittingStartTimes;
            foreach (var s in startTimes)
            {
                if (s.ToString(@"hh\:mm") == expectedStartTime)
                    return true;
            }
            return false;
        }

        public bool CheckEndTime(Sitting sitting, string expectedEndTime)
        {
            var endTimes = sitting.SittingEndTimes;
            foreach (var s in endTimes)
            {
                if (s.ToString(@"hh\:mm") == expectedEndTime)
                    return true;
            }
            return false;
        }

        //needs modification
        public async Task MembershipAndEmailValidation(CreateReservation m, Data.Reservation reservation)
        {
           
                var custEntered = new Data.Customer
                {
                    CustFName = m.Customer.CustFName,
                    CustLName = m.Customer.CustLName,
                    CustEmail = m.Customer.CustEmail,
                    CustPhone = m.Customer.CustPhone
                };
                //previously unregistered customer will update the info and booking confirmed
                reservation.Customer = await _customerService.UpsertCustomerAsync(custEntered,true, false);
            
        }
        #endregion
    }
}
