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
                .Include(su=>su.Reservation)
                .Include(su => su.Sitting).OrderBy(su=>su.Sitting.Date)
                .ToListAsync();
            return View(sittingUnits);
        }


        public async Task<IActionResult> Allocate(int? id)
        {
            var m = new Models.SittingUnit.Allocate();


            if (id.HasValue)
            {
                m.Reservation = await _cxt.Reservations.Include(r=>r.Sitting).ThenInclude(s=>s.SittingCategory).FirstOrDefaultAsync();
                m.SittingUnits = new MultiSelectList(_cxt.SittingUnits.Where(su => su.SittingId == m.Reservation.SittingId).ToArray(), nameof(SittingUnit.Id), nameof(SittingUnit.TableId));
            }

            return View(m);
        }
    }
}
