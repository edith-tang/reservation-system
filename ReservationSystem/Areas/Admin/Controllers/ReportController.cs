using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Areas.Admin.Models.Report;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Controllers
{
    public class ReportController : AdminAreaBaseController
    {
        public ReportController(ApplicationDbContext cxt, UserManager<IdentityUser> userManager) : base(cxt, userManager)
        {
        }

        public async Task<IActionResult> IndexReport()
        {
            var m = await ReportSitting();
            return View(m);
        }

        #region REPORT METHODS

        public async Task<ReportSitting> ReportSitting()
        {
            var today = DateTime.Today;
            var lastMonth = DateTime.Today.AddDays(-30);
            var sittings = await _cxt.Sittings.Include(s => s.SittingCategory).Include(s => s.Reservations)
                .Where(s => s.Date >= lastMonth && s.Date <= today).ToListAsync();

            //sitting(s) with the highest occupancy
            var max = sittings.Max(s => s.Occupancy);
            var maxOcpySittings = sittings.FindAll(s => s.Occupancy == max);

            //average occupancy
            int totalUsedCapacity = 0;
            int totalCapacity = 0;
            foreach (var s in sittings)
            {
                totalUsedCapacity += s.UsedCapacity;
                totalCapacity += s.Capacity;
            }
            var avgOccupancy = (totalUsedCapacity / totalCapacity).ToString("0.00%");

            var model = new ReportSitting();
            model.AvgOcpy = avgOccupancy;
            model.MaxOcpySittings = maxOcpySittings;
            model.HighestOcpy = max.ToString("0.00%");

            return model;
        }

        //public async Task ReportDiningDuration()
        //{
        //    var today = DateTime.Today;
        //    var lastMonth = DateTime.Today.AddDays(-30);
        //    var reservations = await _cxt.Reservations.Include(r => r.Sitting)
        //        .Where(r => r.Sitting.Date >= lastMonth && r.Sitting.Date <= today).ToListAsync();
        //}

        #endregion


    }
}
