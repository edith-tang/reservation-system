﻿using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystem.Data;
using ReservationSystem.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Models.Sitting
{
    public class CreateS
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int SittingCategoryId { get; set; }
        public SelectList SittingCategories { get;  set; }

        public Data.SittingCategory SittingCategory { get; set; }

        //public List<Reservation> Reservations { get; protected set; }
        //public List<SittingUnit> SittingUnits { get; protected set; }


        //public int Capacity { get => SittingCategory.Capacity; }
        //public int UsedCapacity
        //{
        //    get
        //    {
        //        int usedCapacity = 0;
        //        foreach (var r in Reservations)
        //        {
        //            usedCapacity += r.NumOfGuests;
        //        }
        //        return usedCapacity;
        //    }
        //}
        //public int RemainingCapacity { get => SittingCategory.Capacity - UsedCapacity; }
    }
}