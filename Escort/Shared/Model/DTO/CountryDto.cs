using Microsoft.AspNetCore.Http;
using Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.DTO
{
    public class CountryDto
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
