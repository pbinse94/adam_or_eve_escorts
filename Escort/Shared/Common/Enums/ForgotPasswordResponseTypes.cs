using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common.Enums
{
    public enum ForgotPasswordResponseTypes
    {
        TokenUpdatedFailure = 0,
        TokenUpdatedSuccess = 1,
        ForgotEmailAlreadySent = 2,
    }
}
