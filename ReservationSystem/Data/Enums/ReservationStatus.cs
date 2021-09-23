using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Data.Enums
{
    public enum ReservationStatus
    {
        //New ,
        Pending = 0,
        Confirmed = 1,
        //Modified ,
        Cancelled = 2,
        Seated=3,
        Completed = 4,
        Expired=5
    }

}
