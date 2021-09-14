using ReservationSystem.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReservationSystem.Data;

namespace ReservationSystem.Models.SittingCategory
{
    public class DetailsSC
    {


        public Data.SittingCategory SittingCategory { get; set; }
        public List<SCTimeslot> SCTimeslots { get; set; }
        public List<SCTable> SCTables { get; set; }
        public List<Sitting> SCSittings { get; set; }
    }
}
