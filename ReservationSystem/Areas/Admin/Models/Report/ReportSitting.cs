using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ReservationSystem.Data;

namespace ReservationSystem.Areas.Admin.Models.Report
{
    public class ReportSitting
    {        
        public List<Data.Sitting> MaxOcpySittings { get; set; }

        [Display(Name = "Last 30 days Average Occupancy")]
        public string AvgOcpy { get; set; }

        [Display(Name = "Last 30 days Highest Occupancy")]
        public string HighestOcpy { get; set; }
    }
}
