using ReservationSystem.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Models.Sitting
{
    public class QuerySitting
    {
        public int Id { get; set; }        
        public DateTime Date { get; set; }
        public SittingStatus Status { get; set; }

        public int SittingCategoryId { get; set; }
        public Data.SittingCategory SittingCategory { get; set; }
        public List<Data.Reservation> Reservations { get; set; }
        public List<Data.SittingUnit> SittingUnits { get; set; }

        public int Capacity { get => SittingCategory.Capacity; }
        public int UsedCapacity
        {
            get
            {
                if (Reservations.Count > 0)
                { return Reservations.Sum(r => r.NumOfGuests); }
                else { return 0; }
            }
        }
        public int RemainingCapacity { get => Capacity - UsedCapacity; }
    }
}
