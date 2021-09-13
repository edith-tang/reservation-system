using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem.Data.Enums
{
    public enum ReservationStatus
    {
        New = 0,
        Pending = 1,
        Confirmed = 2,
        Modified = 3,
        Cancelled = 4,
        Completed = 5,
    }

}
