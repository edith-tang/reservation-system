using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystem.Data;
using ReservationSystem.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Models.Reservation
{
    //for customer
    public class CreateReservation
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime MaxDate { get; set; }
        public string SysDateFormat { get; set; }
        public List<FutureSitting> FutureSittings { get; set; }
        public int SelectedSittingId { get; set; }
        public FutureSitting SelectedSitting { get; set; }

        public int NumOfGuests { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan EndTime { get => StartTime + Duration; }
        public string Notes { get; set; }

        //public DateTime TimeOfBooking { get; set; }
        //public WayOfBooking WayOfBooking { get; set; }
        //public ReservationStatus Status { get; set; }
    }

    public class FutureSitting
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string SCName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
