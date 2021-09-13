﻿using AutoMapper;
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
        private IMapper _mapper;
        public SittingCategoryController(ApplicationDbContext cxt, IMapper mapper)
        {
            _cxt = cxt;
            _mapper = mapper;
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

                var id = GetId();
                await CreateSCTimeslots(m.StartTime, m.Duration, new TimeSpan(m.IntervalHours, m.IntervalMinutes,0), id);
                await CreateSCTables(id, m.TablesId);
                
            }
            m.Tables = new MultiSelectList(_cxt.Tables.ToArray(), nameof(Table.Id), nameof(Table.Name));
            return View(m);
        }

        #region METHODS
        //generate id for a new sitting category
        public int GetId()
        {
            return _cxt.SittingCategories.Max(s => s.Id) ;
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
                    tStartTime=tStartTime.Add(interval);
                    tEndTime=tEndTime.Add(interval);
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
                tList.Add(new SCTable(sittingCategoryId,ti));
            }
            //for (int i = 0; i < tablesId.Length; i++)
            //{
            //    _cxt.SCTables.Add(new SCTable(sittingCategoryId, tablesId[i]));
            //}
            
            _cxt.SCTables.AddRange(tList);
            await _cxt.SaveChangesAsync();
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
