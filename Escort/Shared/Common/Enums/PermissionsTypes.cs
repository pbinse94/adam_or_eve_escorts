using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common.Enums
{
    public enum PermissionTypes
    {
        [Description("Add")]
        Add = 1,
        [Description("Update")]
        Update = 2,
        [Description("View")]
        View = 3,
        [Description("Delete")]
        Delete = 4,
    }
}
