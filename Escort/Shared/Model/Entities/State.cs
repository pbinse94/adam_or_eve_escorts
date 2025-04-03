using Shared.Model.Base;
using Shared.Model.DTO;

namespace Shared.Model.Entities
{
    public class State
    {
#nullable disable
        public int StateID { get; set; }
        public int CountryID { get; set; }
        public string StateName { get; set; }
        public string Abbreviation { get; set; }
        public DateTime AddedDate { get; set; }

    }
}
