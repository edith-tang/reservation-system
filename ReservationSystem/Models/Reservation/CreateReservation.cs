using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystem.Data;
using ReservationSystem.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Models.Reservation
{
    public class CreateReservation
    {
        public int Id { get; set; }
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
        public Customer Customer { get;  set; }
        public Sitting Sitting { get; set; }

        //public List<SittingUnit> SeatAllocations { get; set; }
        #endregion

        public SelectList AllSittings { get; set; }
        public SelectList Dates { get; set; }
        public DateTime Date { get; set; }
        //public SelectList SittingsPerDate { get { } }

        //public void FindSittingsByDate(DateTime date)
        //{
        //    AllSittings.FindAll(s => s.Date == date);
        //}

    }
}
