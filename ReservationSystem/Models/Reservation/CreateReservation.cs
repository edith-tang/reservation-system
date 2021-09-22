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
        public DateTime MaxDate { get; set; }
        public string SysDateFormat { get; set; }

        public CustomerDTO Customer { get; set; }

        #region RESERVATION INFO
        public int SelectedSittingId { get; set; }
        public string SelectedDate { get; set; }        
        public string ExpectedStartTime { get; set; }
        public string ExpectedEndTime { get; set; }
        public int NumOfGuests { get; set; }
        public string Notes { get; set; }
        #endregion
    }

    public class SittingDTO
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string SCName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public class CustomerDTO
    {
        public string CustFName { get; set; }
        public string CustLName { get; set; }
        public string CustEmail { get; set; }
        public string CustPhone { get; set; }
    }
}
