using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common.Enums
{
    public enum PaymentReport
    {
        [Description("Paid")]
        Paid = 1,
        [Description("UnPaid")]
        UnPaid = 2
    }
}