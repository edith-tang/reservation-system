using AutoMapper;
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
    public class SittingCategoryController : Controller
    {
        private readonly ApplicationDbContext _cxt;
        private readonly IMapper _mapper;
        public SittingCategoryController(ApplicationDbContext cxt, IMapper mapper)
        {
            _cxt = cxt;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index()
        {
            var sittingCategories = await GetSittingCategories();
            return View(sittingCategories);
        }

        //public async Task<ActionResult> DetailsSC_old(int id)
        //{

        //    ViewBag.SCTimeslots = await GetSCTimeslots(id);
        //    ViewBag.SCTables = await GetSCTables(id);
        //    ViewBag.SCSittings = await GetSCSittings(id);
        //    return View(ViewBag);
        //}

        public async Task<ActionResult> DetailsSC(int id)
        {
            var m = new Models.SittingCategory.DetailsSC
            {
                SittingCategory = await GetSittingCategoryById(id),
                SCTimeslots = await GetSCTimeslots(id),
                SCTables = await GetSCTables(id),
                SCSittings = await GetSCSittings(id),
            };

            return View(m);
        }

        [HttpGet]
        public ActionResult CreateSC()
        {
            var m = new Models.SittingCategory.CreateSC
            {
                Tables = new MultiSelectList(_cxt.Tables.ToArray(), nameof(Table.Id), nameof(Table.Name))
            };
            return View(m);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSC(Models.SittingCategory.CreateSC m)
        {
            if (ModelState.IsValid)
            {
                var s = _mapper.Map<SittingCategory>(m);
                _cxt.SittingCategories.Add(s);
                await _cxt.SaveChangesAsync();

                var id = await GetSittingCategoryId();
                await CreateSCTimeslots(m.StartTime, m.Duration, new TimeSpan(m.IntervalHours, m.IntervalMinutes, 0), id);
                await CreateSCTables(id, m.TablesId);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                m.Tables = new MultiSelectList(_cxt.Tables.ToArray(), nameof(Table.Id), nameof(Table.Name));
                return View(m);
            }
        }

        #region SC METHODS
        //generate id for a new sitting category
        public async Task<int> GetSittingCategoryId()
        {
            return await _cxt.SittingCategories.MaxAsync(s => s.Id);
        }

        //create a list of timeslots for a sitting category
        public async Task CreateSCTimeslots(TimeSpan startTime, TimeSpan duration, TimeSpan interval, int sittingCategoryId)
        {
            var tList = new List<SCTimeslot>();
            if (duration.TotalMinutes % interval.TotalMinutes == 0)
            {
                var tStartTime = startTime;
                var tEndTime = startTime.Add(interval);
                for (int i = 0; i < duration.TotalMinutes / interval.TotalMinutes; i++)
                {
                    tList.Add(new SCTimeslot(sittingCategoryId, tStartTime, tEndTime));
                    tStartTime = tStartTime.Add(interval);
                    tEndTime = tEndTime.Add(interval);
                }
                _cxt.SCTimeslots.AddRange(tList);
                await _cxt.SaveChangesAsync();
            }
            else
            {
                throw new Exception("not permitted for non-interger number");
            }
        }

        //create a list of tables for a sitting category
        public async Task CreateSCTables(int sittingCategoryId, int[] tablesId)
        {

            var tList = new List<SCTable>();
            foreach (var ti in tablesId)
            {
                tList.Add(new SCTable(sittingCategoryId, ti));
            }
            _cxt.SCTables.AddRange(tList);
            await _cxt.SaveChangesAsync();
        }

        //load all exisitng SCs
        public async Task<List<SittingCategory>> GetSittingCategories()
        {
            return await _cxt.SittingCategories.ToListAsync();
        }

        public async Task<SittingCategory> GetSittingCategoryById(int sittingCategoryId)
        {
            return await _cxt.SittingCategories.FirstOrDefaultAsync(s => s.Id == sittingCategoryId);
        }
        public async Task<SittingCategory> GetSittingCategoryByName(string name)
        {
            return await _cxt.SittingCategories.FirstOrDefaultAsync(s => s.Name == name);
        }

        //load a list of timeslots
        public async Task<List<SCTimeslot>> GetSCTimeslots(int sittingCategoryId)
        {
            return await _cxt.SCTimeslots.Where(s => s.SittingCategoryId == sittingCategoryId).ToListAsync();
        }

        //load a list of tables
        public async Task<List<SCTable>> GetSCTables(int sittingCategoryId)
        {
            return await _cxt.SCTables.Where(s => s.SittingCategoryId == sittingCategoryId).Include(s => s.Table).OrderBy(s => s.TableId).ToListAsync();
        }

        //load a list of sittings
        public async Task<List<Sitting>> GetSCSittings(int sittingCategoryId)
        {
            return await _cxt.Sittings.Where(s => s.SittingCategoryId == sittingCategoryId).ToListAsync();
        }
        #endregion
    }
}
