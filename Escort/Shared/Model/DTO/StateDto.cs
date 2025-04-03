using Microsoft.AspNetCore.Http;
using Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model.DTO
{
    public class StateDto
    {
#nullable disable
        public int StateID { get; set; }
        public int CountryID { get; set; }
        public string StateName { get; set; }
        public string Abbreviation { get; set; }
        public DateTime AddedDate { get; set; }

    }
}
