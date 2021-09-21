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
    //reservation made by customer
    public class CreateReservation
    {
        public List<SittingOption> SittingOptions { get; set; }
        public int SelectedSittingOptionId { get; set; }
        public SittingOption SelectedSittingOption { get; set; }

        public DateTime MaxDate { get; set; }
        public string SysDateFormat { get; set; }

        public Customer Customer { get; set; }

        #region RESERVATION INFO
        public int NumOfGuests { get; set; }
        public string Notes { get; set; }
        public TimeSpan ExpectedStartTime { get; set; }
        public List<SCTimeslot> PreferredTimeslots { get; set; }
        #endregion
    }

    public class SittingOption
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string SCName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public class Customer
    {
        public string CustFName { get; set; }
        public string CustLName { get; set; }
        public string CustEmail { get; set; }
        public string CustPhone { get; set; }
    }
}
