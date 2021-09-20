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
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan EndTime { get => StartTime + Duration; }
        public string Notes { get; set; }

        public DateTime TimeOfBooking { get; set; }
        public WayOfBooking WayOfBooking { get; set; }
        public ReservationStatus Status { get; set; }

        #region RELATIONSHIPS
        public Customer Customer { get; private set; }
        public Sitting Sitting { get; set; }
        //public List<SittingUnit> SeatAllocations { get; set; }
        #endregion

        //#region CONSTRUCTOR
        //public Reservation(int customerId, int sittingId, int numOfGuests, TimeSpan startTime, TimeSpan duration, 
        //    string notes, DateTime timeOfBooking, int wayOfBooking, int status)
        //{
        //    CustomerId = customerId;
        //    SittingId = sittingId;
        //    NumOfGuests = numOfGuests;
        //    StartTime = startTime;
        //    Duration = duration;
        //    Notes = notes;
        //    TimeOfBooking = timeOfBooking;
        //    WayOfBooking = (WayOfBooking)wayOfBooking;
        //    Status = (ReservationStatus)status;
        //}
        //#endregion
    }
}
