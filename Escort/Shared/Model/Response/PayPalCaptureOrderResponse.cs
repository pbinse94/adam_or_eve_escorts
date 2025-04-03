using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Response
{
    public class PayPalCaptureOrderResponse
    {
        #nullable disable
        public string Id { get; set; }
        public string Status { get; set; }
    }
}
