using System.ComponentModel;

namespace Shared.Common.Enums
{
    public enum DayOfWeekType : byte
    {
        [Description("Sunday")]
        Sunday = 1,

        [Description("Monday")]
        Monday,

        [Description("Tuesday")]
        Tuesday,

        [Description("Wednesday")]
        Wednesday,

        [Description("Thursday")]
        Thursday,

        [Description("Friday")]
        Friday,

        [Description("Saturday")]
        Saturday
    }
}
