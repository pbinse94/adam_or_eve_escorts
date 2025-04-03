using Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Request.Admin
{
    public class UserTokenTransactionRequestModel : DataTableParameters
    {
        public int UserId { get; set; }
        public int ClientId { get; set; }

        public int FilterBy { get; set; }
        public string? FromDate { get; set; }

        public string? ToDate { get; set; }
    }
}
