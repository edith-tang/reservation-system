using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Areas.Admin.Models.SittingCategory
{
    public class CreateSC
    {       
        public string Name { get;  set; }
        public int Capacity { get;  set; }

        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get;  set; }
        public TimeSpan Duration { get;  set; }

        [Display(Name = "End Time")]
        public TimeSpan EndTime { get => StartTime + Duration; }

        [Display(Name = "Sitting duration (hours)")]
        public int IntervalHours { get;  set; }

        [Display(Name = "Sitting duration (minutes)")]
        public int IntervalMinutes { get;  set; }

        [Display(Name = "Tables")]
        public int[] TablesId { get; set; }
        public MultiSelectList Tables { get; set; }
    }
}
