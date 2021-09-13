using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data.Users;

namespace ReservationSystem.Data
{
    public class Customer
    {
        public int Id { get; private set; }
        public string CustFName { get; set; }
        public string CustLName { get; set; }
        public string CustEmail { get; set; }
        public string CustPhone { get; set; }
        //public int MemberId { get; set; }

        #region RELATIONSHIPS        
        public List<Reservation> Reservations { get; private set; }
        //public Member Member { get; set; }
        #endregion
    }
}
