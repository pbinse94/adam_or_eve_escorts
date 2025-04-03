using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Response
{
    public class PaypalOrderResponse
    {
        #nullable disable
        public string Id { get; set; }

        public string Status { get; set; }
        public string ApproveLink { get; set; }
    }

    public class Links
    {
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Method { get; set; }
    }


}
