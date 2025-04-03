using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Entities
{
    public class EscortLocation
    {
        public int ID { get; set; }
        public int EscortId { get; set; }
        public byte AddressType { get; set; }
        public int State { get; set; }
        public int City { get; set; }
    }
}
