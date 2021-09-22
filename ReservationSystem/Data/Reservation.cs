using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ReservationSystem.Data.Enums;

namespace ReservationSystem.Data
{
    public class Reservation
    {
        public int Id { get; private set; }        
        public int CustomerId { get; set; }
        public int SittingId { get; set; }
        public int NumOfGuests { get; set; }
        public TimeSpan ExpectedStartTime { get; set; }
        public TimeSpan ExpectedEndTime { get; set; }
        public string Notes { get; set; }

        public DateTime TimeOfBooking { get; set; }
        public WayOfBooking WayOfBooking { get; set; }
        public ReservationStatus Status { get; set; }

        #region RELATIONSHIPS
        public Customer Customer { get; set; }
        public Sitting Sitting { get; set; }
        public List<SittingUnit> AllocatedSUs { get; set; }
        #endregion
    }
}
