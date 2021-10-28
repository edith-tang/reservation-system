using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Data;
using ReservationSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SittingsController : ControllerBase
    {
        #region DI
        private readonly ApplicationDbContext _cxt;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CustomerService _customerService;
        public SittingsController(ApplicationDbContext cxt, UserManager<IdentityUser> userManager, CustomerService customerService)
        {
            _cxt = cxt;
            _userManager = userManager;
            _customerService = customerService;
        }

        public UserManager<IdentityUser> UserManager => _userManager;
        #endregion

        // GET: api/reservations
        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }

    }
}
