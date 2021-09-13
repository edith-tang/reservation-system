﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReservationSystem.Data.Enums;

namespace ReservationSystem.Data
{
    public class SittingUnit
    {
        public int Id { get; private set; }
        public int SittingId { get; private set; }        
        public int TimeslotId { get; set; }
        public int TableId { get; set; }
        public SittingUnitStatus Status { get; set; }
        public int? ReservationId { get; set; }

        #region RELATIONSHIPS
        public Sitting Sitting { get; private set; }
        public Reservation Reservation { get; set; }
        #endregion

        #region CONSTRUCTOR
        public SittingUnit(int sittingId)
        {
            SittingId = sittingId;
            Status = SittingUnitStatus.Available;
        }

        //public SittingUnit(int sittingId, int? reservationId, int status, int timeslotId, int tableId)
        //{
        //    SittingId = sittingId;
        //    if (reservationId.HasValue) { ReservationId = reservationId.Value; }
        //    Status = (SittingUnitStatus)status;
        //    TimeslotId = timeslotId;
        //    TableId = tableId;
        //}
        #endregion
    }
}
