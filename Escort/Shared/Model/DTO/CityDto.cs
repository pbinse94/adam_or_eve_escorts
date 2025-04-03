using Microsoft.AspNetCore.Http;
using Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.DTO
{
    public class CityDto
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
