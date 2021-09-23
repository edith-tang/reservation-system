using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Models.SittingUnit;
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
                m.CurrentReservation = await _cxt.Reservations
                    .Include(r => r.Sitting.SittingCategory).ThenInclude(sc => sc.SCTables.OrderBy(t => t.Table.Id)).ThenInclude(sct => sct.Table)
                    .Include(r => r.Sitting.SittingCategory).ThenInclude(sc => sc.SCTimeslots.OrderBy(t => t.StartTime))
                    .Include(r => r.Sitting.SittingUnits)
                    .FirstOrDefaultAsync(r => r.Id == id);

                //get row and colomns
                m.SCTimeslots = m.CurrentReservation.Sitting.SittingCategory.SCTimeslots;
                m.SCTables = m.CurrentReservation.Sitting.SittingCategory.SCTables;
                m.FullSittingUnits = m.CurrentReservation.Sitting.SittingUnits;

                //transferring date into DTO

                foreach (var scts in m.SCTimeslots)
                {
                    m.SCTimeslotsDTO.Add(new SCTimeslotDTO
                    {
                        Id = scts.Id,
                        StartTime = scts.StartTime.ToString(@"hh\:mm\:ss"),
                        EndTime = scts.EndTime.ToString(@"hh\:mm\:ss"),
                    });
                }

                foreach (var sct in m.SCTables)
                {
                    m.SCTablesDTO.Add(new SCTableDTO
                    {
                        Id = sct.Id,
                        Name = sct.Table.Name,
                        Area = sct.Table.Area,
                    });
                }

                foreach (var fsu in m.FullSittingUnits)
                {
                    m.SittingUnitsDTO.Add(new SittingUnitDTO
                    {
                        Id = fsu.Id,
                        TableId = fsu.TableId,
                        TimeslotId = fsu.TimeslotId,
                        ReservationId= fsu.ReservationId,
                    });
                }


                //dropdown list demo
                var availableSittingUnits = _cxt.SittingUnits
                    .Where(su => su.SittingId == m.CurrentReservation.SittingId && su.Status == 0)
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
        public async Task<IActionResult> Allocate(int currentReservationId, int[] selectedSittingUnitId)
        {
            var currentReservation = _cxt.Reservations.FirstOrDefault(r => r.Id == currentReservationId);
            var previousSittingUnits =_cxt.SittingUnits.Where(su => su.ReservationId== currentReservationId).ToList();
            previousSittingUnits.ForEach(su => { su.ReservationId = null; su.Status = Data.Enums.SittingUnitStatus.Available; });

            var currentSittingUnits=_cxt.SittingUnits.Where(su => selectedSittingUnitId.Contains(su.Id)).ToList();
            currentSittingUnits.ForEach(su => { su.ReservationId = currentReservationId; su.Status = Data.Enums.SittingUnitStatus.Reserved; });

            currentReservation.Status = Data.Enums.ReservationStatus.Confirmed;

            await _cxt.SaveChangesAsync();
            return Json(Url.Action("Index", "Reservation"));
        }
    }
}
