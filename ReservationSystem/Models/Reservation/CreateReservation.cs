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

        public int SittingsListId { get; set; }
        public SelectList SittingsList { get; set; }

        public DateTime MaxDate { get; set; }
        public string SysDateFormat { get; set; }

        public List<AvailableSitting> AvailableSittings { get; set; }

        public int NumOfGuests { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan EndTime { get => StartTime + Duration; }
        public string Notes { get; set; }

        public DateTime TimeOfBooking { get; set; }
        public WayOfBooking WayOfBooking { get; set; }
        public ReservationStatus Status { get; set; }
    }

    public class AvailableSitting
    {
        public int Id { get; set; }
        public string Date { get; set; }
    }
}
