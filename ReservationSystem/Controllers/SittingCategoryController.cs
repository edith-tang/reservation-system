using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Controllers
{    
    public class SittingCategoryController : Controller
    {
        private readonly ApplicationDbContext _cxt;
        public SittingCategoryController(ApplicationDbContext cxt)
        {
            _cxt = cxt;
        }

        [HttpGet]
        public ActionResult CreateSC()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateSCAsync(TimeSpan startTime, TimeSpan duration, TimeSpan interval)
        {
            int sittingCategoryId = GetId();
            CreateSCTimeslots(startTime, duration, interval, sittingCategoryId);
            await CreateSCTables(sittingCategoryId);
            return View();
        }

        #region METHODS
        //generate id for a new sitting category
        public int GetId()
        {
            return _cxt.SittingCategories.Max(s => s.Id) + 1;
        }

        //create a list of timeslots for a sitting category
        public List<SCTimeslot> CreateSCTimeslots(TimeSpan startTime, TimeSpan duration, TimeSpan interval, int sittingCategoryId)
        {
            var tList = new List<SCTimeslot>();
            if (duration.Minutes % interval.Minutes == 0)
            {
                var tStartTime = startTime;
                var tEndTime = startTime.Add(interval);
                for (int i = 0; i < duration.Minutes / interval.Minutes; i++)
                {
                    tList.Add(new SCTimeslot(sittingCategoryId, tStartTime, tEndTime));
                    tStartTime.Add(interval);
                    tEndTime.Add(interval);
                }
            }
            else
            {
                throw new Exception("not permitted for non-interger number");
            }
            return tList;
        }

        //create a list of tables for a sitting category
        public async Task<List<SCTable>> CreateSCTables(int sittingCategoryId)
        {
            var tables = await _cxt.Tables.ToListAsync();
            var tList = new List<SCTable>();
            foreach (var t in tables)
            {
                tList.Add(new SCTable(sittingCategoryId, t.Id));
            }
            return tList;
        }

        //load a list of timeslots from database
        public async Task<List<SCTimeslot>> GetSCTimeslots(int sittingCategoryId)
        {
            return await _cxt.SCTimeslots.Where(s => s.SittingCategoryId == sittingCategoryId).ToListAsync();
        }

        //load a list of tables from database
        public async Task<List<SCTable>> GetSCTables(int sittingCategoryId)
        {
            return await _cxt.SCTables.Where(s => s.SittingCategoryId == sittingCategoryId).ToListAsync();
        }
        #endregion
    }
}
