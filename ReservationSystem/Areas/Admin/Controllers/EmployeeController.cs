using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReservationSystem.Data;
using ReservationSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Controllers
{
    public class EmployeeController : AdminAreaBaseControlller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CustomerService _customerService;

        public EmployeeController(ApplicationDbContext cxt,  CustomerService customerService, UserManager<IdentityUser> userManager) : base(cxt)
        {
            _userManager = userManager;
            _customerService = customerService;
        }

        [HttpGet]
        public IActionResult CreateEmployee()
        {
            var m = new Models.Employee.CreateEmployee();
            return View(m);
        }
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(Models.Employee.CreateEmployee m)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = m.Email, Email = m.Email, PhoneNumber = m.Phone };
                var result = await _userManager.CreateAsync(user, m.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Employee");
                    var e = new Data.Employee
                    {
                        EmpFName = m.FName,
                        EmpLName = m.LName,
                        EmpEmail = m.Email,
                        EmpPhone = m.Phone,
                        IdentityUserId = user.Id
                    };
                    
                    _cxt.Employees.Add(e);

                    await _cxt.SaveChangesAsync();

                    return RedirectToAction("Index","Home",new { area="Employee"});


                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(m);
        }
    }
}
