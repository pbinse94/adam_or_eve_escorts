using Shared.Model.Base;
using Shared.Model.DTO;

namespace Shared.Model.Entities
{
    public class City
    {
        public int CityID { get; set; }
        public int StateID { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string? Banner { get; set; }
        public int OrderNo { get; set; }
        public DateTime AddedDate { get; set; }
        public decimal? MinTemperature { get; set; }
        public decimal? MaxTemperature { get; set; }
        public decimal? Temperature { get; set; }
    }
}
