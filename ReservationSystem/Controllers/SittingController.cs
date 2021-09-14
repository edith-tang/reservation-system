using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
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

        public IActionResult CreateS(int? id)
        {
            var m = new Models.Sitting.CreateS
            {
                SittingCategories = new SelectList(_cxt.SittingCategories.ToArray(), nameof(SittingCategory.Id), nameof(SittingCategory.Name))
            }
            ;
            if (id.HasValue) 
            {
                var sittingCategory = _cxt.SittingCategories.FirstOrDefault(sc => sc.Id == id);
                m.SittingCategoryId = sittingCategory.Id;
                m.SittingCategory = sittingCategory;
            }
            
            return View(m);
        }

        #region SITTING METHODS
        //load all exisitng Sittings
        public async Task<List<Sitting>> GetSittings()
        {
            return await _cxt.Sittings.ToListAsync();
        }


        //add a new sitting to database
        public async Task CreateSitting(int sittingCategoryId)
        {
            //use auto mapper here
            var sitting = new Sitting();
            await _cxt.Sittings.AddAsync(sitting);
            await _cxt.SaveChangesAsync();
        }

        //add sitting units for a sitting to database
        //public async Task CreateSittingUnits(SittingModel sittingModel)
        //{
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
        //    await _cxt.SaveChangesAsync();
        //}

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
