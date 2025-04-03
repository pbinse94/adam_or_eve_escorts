using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.DTO
{
    public class LocationDto
    {
        public int EscortID { get; set; }
        public byte AddressType { get; set; }
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
