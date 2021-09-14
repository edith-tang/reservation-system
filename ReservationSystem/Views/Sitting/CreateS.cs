using ReservationSystem.Data;
using ReservationSystem.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Views.Sitting
{
    public class CreateS
    {


        public int Id { get;  set; }
        public int SittingCategoryId { get;  set; }
        public DateTime Date { get;  set; }
        public SittingStatus Status { get;  set; }

        public SittingCategory SittingCategory { get; protected set; }
        public List<Reservation> Reservations { get; protected set; }
        public List<SittingUnit> SittingUnits { get; protected set; }


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
