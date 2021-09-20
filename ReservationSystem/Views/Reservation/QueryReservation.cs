using ReservationSystem.Data;
using ReservationSystem.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Views.Reservation
{
    public class QueryReservation
    {
        public int Id { get; private set; }
        public int CustomerId { get; set; }
        public int SittingId { get; set; }
        public int NumOfGuests { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan EndTime { get => StartTime + Duration; }
        public string Notes { get; set; }

        public DateTime TimeOfBooking { get; set; }
        public WayOfBooking WayOfBooking { get; set; }
        public ReservationStatus Status { get; set; }

        public Customer Customer { get; private set; }
        public Sitting Sitting { get; set; }
        public List<SittingUnit> SeatAllocations { get; set; }
    }
}
