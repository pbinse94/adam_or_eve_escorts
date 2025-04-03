using Shared.Model.Base;
using Shared.Model.DTO;

namespace Shared.Model.Entities
{
    public class Country
    {
#nullable disable
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public string Abbreviation { get; set; }
        public DateTime AddedDate { get; set; }
        public string CountryCode { get; set; }
        public string CountryCodeImage { get; set; }
        public int SortOrder { get; set; }
    }
}
