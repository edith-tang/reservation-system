using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ReservationSystem.Data.Enums;

namespace ReservationSystem.Data
{
    public class Sitting
    {
        public int Id { get; protected set; }
        public int SittingCategoryId { get; protected set; }
        public DateTime Date { get; protected set; }
        public SittingStatus Status { get; protected set; }

        #region RELATIONSHIPS        
        public SittingCategory SittingCategory { get; protected set; }
        //public List<Reservation> Reservations { get; protected set; }
        //public List<SittingUnit> SittingUnits { get; protected set; }
        #endregion
    }
}
