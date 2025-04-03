using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.DTO
{
    public class GetLastTwelveMonthGiftTokensDto
    {
        public int Year { get; set; }
        public string Month { get; set; } = string.Empty;
        public int Tokens { get; set; }
    }
}
