using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Models.SittingUnit
{
    public class Allocate
    {

        public Data.Reservation Reservation { get; set; }
        public List<SCTimeslot> SCTimeslots { get; set; }

        public List<SCTable> SCTables { get; set; }

        public List<Data.SittingUnit> AvailableSittingUnits { get; set; }

        public List<int> SelectedSittingUnitsId { get; set; }


        public int[] SittingUnitsId { get; set; }
        public MultiSelectList SittingUnits { get; set; }
    }
}
