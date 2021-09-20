﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Data.Enums;
using ReservationSystem.Models;
using ReservationSystem.Models.Sitting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Controllers
{
    public class SittingController : Controller
    {
        private readonly ApplicationDbContext _cxt;
        private readonly IMapper _mapper;
        public SittingController(ApplicationDbContext cxt, IMapper mapper)
        {
            _cxt = cxt;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var sittings = await GetSittings();
            var qSittings = new List<QuerySitting>();
            foreach (var s in sittings)
            {
                QuerySitting qs = _mapper.Map<QuerySitting>(s);
                qSittings.Add(qs);
            }
            return View(qSittings);
        }

        public async Task<IActionResult> CreateSitting(int? id)
        {
            var m = new Models.Sitting.CreateSitting();
            if (id.HasValue)
            {
                var sittingCategory = await _cxt.SittingCategories.FirstOrDefaultAsync(sc => sc.Id == id);
                m.SittingCategoryId = sittingCategory.Id;
                m.SittingCategory = sittingCategory;
            }
            else
            {
                m.SittingCategories = new SelectList(_cxt.SittingCategories.ToArray(), nameof(SittingCategory.Id), nameof(SittingCategory.Name));
            }
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSitting(Models.Sitting.CreateSitting m)
        {            
            var sittingCategory = await _cxt.SittingCategories.FirstOrDefaultAsync(sc => sc.Id == m.SittingCategoryId);
            var sittings = new List<Sitting>();
            DateTime date = m.StartDate;
            var nonValidDates = new List<String>();

            //int days = (m.EndDate - m.StartDate).Days + 1;
            //for (int i = 0; i < days; i++)

            while (date <= m.EndDate)
            {
                if (SCSelectionValidation(date, m.SittingCategoryId))
                {
                    sittings.Add(new Sitting { SittingCategoryId = m.SittingCategoryId, Date = date, Status = SittingStatus.Open });
                }
                else { nonValidDates.Add(date.ToString("dd/MM/yyyy")); }

                date = date.AddDays(1);
            }

            if (nonValidDates.Count != 0)
            { 
                TempData["Message"] = string.Format("Your choice {0} overlaps with existing sittings {1}", 
                    sittingCategory.Name,
                    string.Join(",", nonValidDates)
                    ); 
            }

            _cxt.Sittings.AddRange(sittings);
            await _cxt.SaveChangesAsync();

            //CreateSittingUnits();
            int sittingId = await _cxt.Sittings.MaxAsync(s => s.Id) - sittings.Count + 1;
            for (int i = 0; i < sittings.Count; i++)
            {
                await CreateSittingUnits(sittingId, m.SittingCategoryId);
                sittingId++;
            }

            return RedirectToAction(nameof(Index));
        }

        #region SITTING METHODS
        //load all exisitng Sittings
        public async Task<List<Sitting>> GetSittings()
        {
            return await _cxt.Sittings
                .Include(s => s.SittingCategory)
                .Include(s => s.Reservations)
                .Include(s => s.SittingUnits)
                .ToListAsync();
        }

        //check if the sitting to be created overlaps with existing sittings
        public bool SCSelectionValidation(DateTime date, int sittingCategoryId)
        {
            var sittings = _cxt.Sittings.Where(s => s.Date == date).Include(s => s.SittingCategory).ToList();            
            if (sittings == null) { return true; }

            var sittingCategory = _cxt.SittingCategories.FirstOrDefault(sc => sc.Id == sittingCategoryId);
            foreach (var sitting in sittings)
            {
                if (!(
                    (sittingCategory.EndTime <= sitting.SittingCategory.StartTime) || (sittingCategory.StartTime >= sitting.SittingCategory.EndTime)
                    ))
                { return false; }
            }
            return true;
        }

        //add sitting units for a sitting to database
        public async Task CreateSittingUnits(int sittingId, int sittingCategoryId)
        {
            var sUnits = new List<SittingUnit>();
            var sc = await _cxt.SittingCategories
                .Include(s => s.SCSittings)
                .Include(s => s.SCTables)
                .Include(s => s.SCTimeslots)
                .FirstOrDefaultAsync(s => s.Id == sittingCategoryId);

            foreach (var scTable in sc.SCTables)
            {
                foreach (var scTimeslot in sc.SCTimeslots)
                {
                    sUnits.Add(new SittingUnit(sittingId, scTimeslot.Id, scTable.Id));
                }
            }
            _cxt.SittingUnits.AddRange(sUnits);
                        
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
