using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Entities
{
    public class AvailabilityCalendar
    {
        public int ID { get; set; }
        public int EscortID { get; set; }
        public string? FromTime { get; set; }
        public string? ToTime { get; set; }
        public byte DayNumber { get; set; }
        public bool IsNotAvailable { get; set; }
        public bool IsAvailable24X7 { get; set; }
    }
}
