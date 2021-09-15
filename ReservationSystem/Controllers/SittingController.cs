using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Data.Enums;
using ReservationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Controllers
{
    public class SittingController : Controller
    {
        private readonly ApplicationDbContext _cxt;
        public SittingController(ApplicationDbContext cxt)
        {
            _cxt = cxt;
        }
        public async Task<IActionResult> Index()
        {
            var sittings = await GetSittings();
            return View(sittings);
        }

        public async Task<IActionResult> CreateSitting(int? id)
        {
            var m = new Models.Sitting.CreateSitting
            {
                SittingCategories = new SelectList(_cxt.SittingCategories.ToArray(), nameof(SittingCategory.Id), nameof(SittingCategory.Name))
            };
            if (id.HasValue)
            {
                var sittingCategory = await _cxt.SittingCategories.FirstOrDefaultAsync(sc => sc.Id == id);
                m.SittingCategoryId = sittingCategory.Id;
                m.SittingCategory = sittingCategory;
            }

            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSitting(Models.Sitting.CreateSitting m)
        {
            var sittings = new List<Sitting>();
            var sittingCategory = await _cxt.SittingCategories.FirstOrDefaultAsync(sc => sc.Id == m.SittingCategoryId);
            int days = (m.EndDate - m.StartDate).Days + 1;
            DateTime date = m.StartDate;
            var nonValidDates= new List<String>();
            
            for (int i = 0; i < days; i++)
            {
                if (IsCurrentSittingCategorySelectionValid(date, m.SittingCategoryId))
                {
                    sittings.Add(new Sitting
                    {
                        SittingCategoryId = m.SittingCategoryId,
                        Date = date,
                        Status = SittingStatus.Open,
                    });

                    //CreateSittingUnits();
                }
                else { nonValidDates.Add(date.ToString(("MM/dd/yyyy"))); }
                
                date = date.AddDays(1);

            }
            if (nonValidDates.Count != 0) { TempData["Message"] = String.Format("Selected Sitting Catagory {0} is not created for date {1} because of overlapping with existing sittings.", sittingCategory.Name,String.Join(",", nonValidDates)); }

            _cxt.Sittings.AddRange(sittings);
            await _cxt.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region SITTING METHODS
        //load all exisitng Sittings
        public async Task<List<Sitting>> GetSittings()
        {
            return await _cxt.Sittings.Include(s => s.SittingCategory).ToListAsync();
        }

        //check whether the selected sittingcategory for a new sitting is overlapped with other sittings for the same date
        public bool IsCurrentSittingCategorySelectionValid(DateTime date, int sittingCategoryId)
        {
            var sittings = _cxt.Sittings.Where(s => s.Date == date).Include(s => s.SittingCategory).ToList();
            var sittingCategory = _cxt.SittingCategories.FirstOrDefault(sc => sc.Id == sittingCategoryId);
            if (sittings == null) { return true; }
            foreach (var sitting in sittings)
            {
                if (!(
                    (sittingCategory.EndTime <= sitting.SittingCategory.StartTime)
                    || (sittingCategory.StartTime >= sitting.SittingCategory.EndTime)
                    ))
                { return false; }
            }
            return true;
        }

        //add sitting units for a sitting to database
        public async Task CreateSittingUnits(SittingModel sittingModel)
        {
            //    var sUnits = new List<SittingUnit>();
            //    var sUnit = new SittingUnit(sittingModel.Id);
            //    foreach (var ts in sittingModel.Timeslots)
            //    {
            //        sUnit.TimeslotId = ts.Id;
            //        foreach (var tb in sittingModel.Tables)
            //        {
            //            sUnit.TableId = tb.Id;
            //            sUnits.Add(sUnit);
            //        }
            //    }
            //    await _cxt.SittingUnits.AddRangeAsync(sUnits);
            await _cxt.SaveChangesAsync();
        }

        //find a sitting from database
        public async Task<Sitting> FindSitting(DateTime date)
        {
            //filter sittings by date
            var sittings = await _cxt.Sittings.Where(s => s.Date == date).ToListAsync();
            //add filter for sittings to locate the sitting
            var sitting = new SittingModel();
            return sitting;
        }
        #endregion
    }
}
