using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ReservationSystem.Data.Enums;

namespace ReservationSystem.Data
{
    public class Sitting
    {
        public int Id { get; set; }
        public int SittingCategoryId { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Date { get; set; }
        public SittingStatus Status { get; set; }

        #region RELATIONSHIPS
        public SittingCategory SittingCategory { get; set; }
        public List<Reservation> Reservations { get; set; }
        public List<SittingUnit> SittingUnits { get; set; }
        #endregion

        #region GET ONLY PROPERTIES
        public int Capacity { get => SittingCategory.Capacity; }
        public int UsedCapacity
        {
            get
            {
                return Reservations.Any(r => r.Status != ReservationStatus.Cancelled) ? Reservations.Sum(r => r.NumOfGuests) : 0;
            }
        }
        public int RemainingCapacity { get => Capacity - UsedCapacity; }
        public decimal Occupancy { get => decimal.Round(UsedCapacity / Capacity, 2); }
        public List<TimeSpan> SittingStartTimes { 
            get
            {
                var startTimes = new List<TimeSpan>();
                foreach (var t in SittingCategory.SCTimeslots)
                {
                    startTimes.Add(t.StartTime);
                }
                return startTimes;
            } 
        }
        public List<TimeSpan> SittingEndTimes
        {
            get
            {
                var endTimes = new List<TimeSpan>();
                foreach (var t in SittingCategory.SCTimeslots)
                {
                    endTimes.Add(t.EndTime);
                }
                return endTimes;
            }
        }

        #endregion
    }
}
