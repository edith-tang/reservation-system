using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Models.SittingCategory
{
    public class CreateSC
    {       
        public string Name { get;  set; }
        public int Capacity { get;  set; }
        public TimeSpan StartTime { get;  set; }
        public TimeSpan Duration { get;  set; }
        public TimeSpan EndTime { get => StartTime + Duration; }

        public int IntervalHours { get;  set; }
        public int IntervalMinutes { get;  set; }

        public int[] TablesId { get; set; }
        public MultiSelectList Tables { get; set; }
    }
}
