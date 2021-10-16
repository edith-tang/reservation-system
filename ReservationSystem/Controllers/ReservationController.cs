using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReservationSystem.Data;
using ReservationSystem.Models.Reservation;
using ReservationSystem.Services;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CustomerService _customerService;
        public ReservationController(ApplicationDbContext cxt, UserManager<IdentityUser> userManager, CustomerService customerService)
        {
            _cxt = cxt;
            _userManager = userManager;
            _customerService = customerService;
        }
        #endregion

        #region ACTION METHODS

        [Authorize(Roles = "Member")]
        public async Task<ActionResult> HistoryReservation()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var custAuthenticated = await _cxt.Customers.FirstOrDefaultAsync(c => c.IdentityUserId == user.Id);
            var reservation = await _cxt.Reservations
                .Include(r => r.Sitting)
                .Include(r => r.Customer).Where(r => r.CustomerId == custAuthenticated.Id)
                .OrderBy(r => r.Sitting.Date)
                .ToListAsync();
            return View(reservation);
        }

        [Authorize(Roles = "Member")]
        public async Task<ActionResult> DetailsReservation(int id)
        {
            var reservation = await _cxt.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Sitting.SittingCategory)
                .FirstOrDefaultAsync(r => r.Id == id);
            return View(reservation);
        }

        [HttpGet]
        public async Task<ActionResult> CreateReservation()
        {
            var sittings = await GetAllFutureSittings();
            var m = new CreateReservation
            {
                MaxDate = sittings.Max(s => s.Date).ToString("yyyy-MM-dd"),
                MinDate = DateTime.Today.ToString("yyyy-MM-dd"),
                Customer = new CustomerDTO(),
            };
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var custAuthenticated = await _cxt.Customers.FirstOrDefaultAsync(c => c.IdentityUserId == user.Id);
                m.CustomerId = custAuthenticated.Id;
                m.Customer.CustFName = custAuthenticated.CustFName;
                m.Customer.CustLName = custAuthenticated.CustLName;
                m.Customer.CustEmail = custAuthenticated.CustEmail;
                m.Customer.CustPhone = custAuthenticated.CustPhone;
            }
            return View(m);
        }

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
                        Notes = m.Notes ?? "N/A",
                        TimeOfBooking = DateTime.Now,
                        WayOfBooking = "Online",
                        Status = Data.Enums.ReservationStatus.Pending,
                    };

                    await MembershipAndEmailValidation(m, reservation);

                    _cxt.Reservations.Add(reservation);
                    await _cxt.SaveChangesAsync();
                    
                    return RedirectToAction("ThankYouPage", "Home");

                }
                catch (Exception)
                {
                    //input inconsistent with database
                    //ModelState.AddModelError("Error", "SQL Error!");
                }
            }
            return View(m);
        }

        public async Task<IActionResult> CancelReservation(int id)
        {
            var r = _cxt.Reservations.FirstOrDefault(r => r.Id == id);
            r.Status = Data.Enums.ReservationStatus.Cancelled;
            await _cxt.SaveChangesAsync();
            return RedirectToAction(nameof(HistoryReservation));
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
            return await _cxt.Sittings.Where(s => s.Status != Data.Enums.SittingStatus.Past)
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
        
        public async Task MembershipAndEmailValidation(CreateReservation m, Data.Reservation reservation)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var custAuthenticated = await _cxt.Customers.FirstOrDefaultAsync(c => c.IdentityUserId == user.Id);
                reservation.Customer = custAuthenticated;
            }
            else //For non-member
            {
                var custEntered = new Data.Customer
                {
                    CustFName = m.Customer.CustFName,
                    CustLName = m.Customer.CustLName,
                    CustEmail = m.Customer.CustEmail,
                    CustPhone = m.Customer.CustPhone
                };
                //info of unregistered customer with existing booking records will be updated
                reservation.Customer = await _customerService.UpsertCustomerAsync(custEntered, true,false);
            }
        }
        #endregion
    }
}
