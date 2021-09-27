using Microsoft.AspNetCore.Identity;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Services
{
    public class CustomerService
    {
        private readonly ApplicationDbContext _cxt;

        private readonly UserManager<IdentityUser> _userManager;

        public CustomerService(ApplicationDbContext cxt, UserManager<IdentityUser> userManager)
        {
            _cxt = cxt;
            _userManager = userManager;
        }


    }
}
