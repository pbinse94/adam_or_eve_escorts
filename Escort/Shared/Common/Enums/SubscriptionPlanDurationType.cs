using System.ComponentModel;

namespace Shared.Common.Enums
{
    public enum SubscriptionPlanDurationType
    {
        [Description("Basic Package")]
        IndependentEscortBasic = 1,   // Basic Package  for Free
        [Description("Package 1")]
        IndependentEscortWeeklyStarter = 2,   // Package 1 for 1 week
        [Description("Package 2")]
        IndependentEscortMonthly = 3,       // Package 2 for 4 week
        [Description("Package 3")]
        IndependentEscortQuarterly = 4, // Package 3 for 12 weeks
        [Description("Package 4")]
        IndependentEscortYearly = 5, // Package 4 for 52 weeks

        [Description("Package 1")]
        EstablishmentWeeklyStarter = 6,   // Package 1 for 1 week
        [Description("Package 2")]
        EstablishmentWeeklyPro = 7,       // Package 2 for 1 week
        [Description("Package 3")]
        EstablishmentMonthly = 8, // Package 3 for 4 weeks
        [Description("Package 4")]
        EstablishmentQuarterMonthly = 9, // Package 4 for 12 weeks

    }
}
