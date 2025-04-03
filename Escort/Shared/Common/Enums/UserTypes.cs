using System.ComponentModel;

namespace Shared.Common.Enums
{
    public enum UserTypes
    {
        [Description("Super Admin")]
        SuperAdmin = 1,
        [Description("Client")]
        Client = 2,
        [Description("Independent Escort")]
        IndependentEscort = 3,
        [Description("Establishment")]
        Establishment = 4,
        [Description("Establishment Escort")]
        EstablishmentEscort = 5,

        [Description("Admin")]
        Admin = 6, 
        [Description("Editor")]
        Editor = 7, 
        [Description("Viewer")]
        Viewer = 8,
        [Description("Management")]
        Management = 9,
        [Description("Accounting")]
        Accounting = 10
    }

    public enum DeviceTypeEnum
    {
        [Description("Android")]
        Android = 1,

        [Description("IOS")]
        IOS = 2,

        [Description("Web")]
        Web = 3,
    }


}
