using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReservationSystem.Data.Enums;
using ReservationSystem.Data;

namespace ReservationSystem.Models
{
    public class SittingModel: Sitting
    {
        //public List<SCTimeslot> Timeslots { get => SittingCategory.SCTimeslots; }
        //public List<SCTable> Tables { get => SittingCategory.SCTables; }
        public int Capacity { get => SittingCategory.Capacity; }
        public int UsedCapacity
        {
            get
            {
                int usedCapacity = 0;
                foreach (var r in Reservations)
                {
                    usedCapacity += r.NumOfGuests;
                }
                return usedCapacity;
            }
        }
        public int RemainingCapacity { get => SittingCategory.Capacity - UsedCapacity; }
    }
}
