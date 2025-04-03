using System.ComponentModel;

namespace Shared.Common.Enums
{
    public enum UserTokenTransaction
    {
        [Description("Purchase")]
        Purchase = 1,
        [Description("Spent")]
        Spent = 2,
        [Description("Balance")]
        Balance = 3
    }
}
