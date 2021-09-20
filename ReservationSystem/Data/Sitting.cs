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
        public int Id { get; set; }
        public int SittingCategoryId { get; set; }
        public DateTime Date { get; set; }
        public SittingStatus Status { get; set; }

        #region RELATIONSHIPS
        public SittingCategory SittingCategory { get; set; }
        public List<Reservation> Reservations { get; set; }
        public List<SittingUnit> SittingUnits { get; set; }
        #endregion
    }
}
