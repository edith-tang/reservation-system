using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Data.Enums;
using ReservationSystem.Services;

namespace ReservationSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        #region DI
        private readonly ApplicationDbContext _cxt;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CustomerService _customerService;
        public ReservationsController(ApplicationDbContext cxt, UserManager<IdentityUser> userManager, CustomerService customerService)
        {
            _cxt = cxt;
            _userManager = userManager;
            _customerService = customerService;
        }
        #endregion


        #region methods

        // GET: api/Reservations
        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            var r = await _cxt.Reservations
                .Include(r => r.Sitting.SittingCategory)
                .Include(r => r.Customer)
                .OrderBy(r => r.Sitting.Date)
                .ToListAsync();

            var reservation = new ReservationDTO
            {

            };
            return Ok(r);
        }


        // GET: api/Reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _cxt.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        #endregion

        public class ReservationDTO
        {
            public int Id { get; private set; }
            public int CustomerId { get; set; }
            public int SittingId { get; set; }

            public int NumOfGuests { get; set; }

            public TimeSpan ExpectedStartTime { get; set; }

            public TimeSpan ExpectedEndTime { get; set; }

            public string Notes { get; set; }

            public DateTime TimeOfBooking { get; set; }

            public string WayOfBooking { get; set; }
            public ReservationStatus Status { get; set; }

            public Customer Customer { get; set; }

            
            public DateTime SittingDate { get; set; }

            public string SittingCategoryName { get; set; }

        }

    }

}
