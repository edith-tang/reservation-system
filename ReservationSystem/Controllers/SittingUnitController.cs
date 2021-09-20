using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Controllers
{
    public class SittingUnitController : Controller
    {

        private readonly ApplicationDbContext _cxt;
        public SittingUnitController(ApplicationDbContext cxt)
        {
            _cxt = cxt;
        }
        public async Task<IActionResult> Index()
        {
            var sittingUnits = await _cxt.SittingUnits
                .Include(su => su.Reservation)
                .Include(su => su.Sitting).OrderBy(su => su.Sitting.Date)
                .ToListAsync();
            return View(sittingUnits);
        }


        public async Task<IActionResult> Allocate(int? id)
        {
            var m = new Models.SittingUnit.Allocate();


            if (id.HasValue)
            {
                m.Reservation = await _cxt.Reservations
                    .Include(r => r.Sitting.SittingCategory).ThenInclude(sc => sc.SCTables.OrderBy(t=>t.Table.Id)).ThenInclude(sct=>sct.Table)
                    .Include(r => r.Sitting.SittingCategory).ThenInclude(sc => sc.SCTimeslots.OrderBy(t=>t.StartTime))
                    .Include(r=>r.Sitting.SittingUnits)
                    .FirstOrDefaultAsync(r=>r.Id==id);

                //get row and colomns
                m.SCTimeslots = m.Reservation.Sitting.SittingCategory.SCTimeslots;
                m.SCTables = m.Reservation.Sitting.SittingCategory.SCTables;
                m.AvailableSittingUnits= m.Reservation.Sitting.SittingUnits;

                var availableSittingUnits = _cxt.SittingUnits
                    .Where(su => su.SittingId == m.Reservation.SittingId)
                    .Select(su => new
                    {
                        Id = su.Id,
                        Description = $"{ su.TableId}-- {su.TimeslotId}"
                    }).ToList();

                m.SittingUnits = new MultiSelectList(availableSittingUnits, "Id", "Description");
            }

            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> Allocate(Models.SittingUnit.Allocate m)
        {

            await _cxt.SaveChangesAsync();
            return View(m);
        }
    }
}
