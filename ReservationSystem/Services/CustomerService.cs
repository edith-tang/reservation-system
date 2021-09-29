using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Customer> UpsertCustomerAsync(Customer data, bool update)
        {
            var customer = await _cxt.Customers.FirstOrDefaultAsync(c => c.CustEmail == data.CustEmail);
            if (customer == null)
            {
                customer = new Customer
                {
                    CustFName = data.CustFName,
                    CustLName = data.CustLName,
                    CustEmail = data.CustEmail,
                    CustPhone = data.CustPhone,
                    IdentityUserId=data.IdentityUserId,

                };
                _cxt.Customers.Add(customer);

            }
            if (customer != null && update)
            {
                customer.CustFName = data.CustFName;
                customer.CustLName = data.CustLName;
                customer.CustPhone = data.CustPhone;
                customer.IdentityUserId = data.IdentityUserId;
            }
            
            return customer;

        }
    }
}
