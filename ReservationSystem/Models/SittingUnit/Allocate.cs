using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Models.SittingUnit
{
    public class Allocate
    {

        public Data.Reservation Reservation { get; set; }

        public int[] SittingUnitsId { get; set; }
        public MultiSelectList SittingUnits { get; set; }
    }
}
