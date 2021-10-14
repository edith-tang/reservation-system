using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Services
{
    public class AdminService
    {
        private readonly ApplicationDbContext _cxt;

        private readonly UserManager<IdentityUser> _userManager;

        public AdminService(ApplicationDbContext cxt, UserManager<IdentityUser> userManager)
        {
            _cxt = cxt;
            _userManager = userManager;
        }

        public async Task<IdentityUser> SeedAdmin()
        {
            var user = await _userManager.FindByNameAsync("admin");
            if (user == null)
            {

                var admin = new IdentityUser { UserName = "admin" };
                var result = await _userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");
                    var emp = new Data.Employee
                    {
                        EmpFName = "admin",
                        EmpLName = "admin",
                        EmpEmail = null,
                        EmpPhone = null,
                        IdentityUserId = admin.Id
                    };
                    _cxt.Employees.Add(emp);
                    await _cxt.SaveChangesAsync();
                }

            }
            return user;
        }
    }
}
