using System.Security.Policy;
using System;

namespace Shared.Common.Enums
{
    public enum PaypalSubscriptionStatus
    {
        /// <summary>
        /// The subscription is created but not yet approved by the buyer.
        /// </summary>
        APPROVAL_PENDING,

        /// <summary>
        /// The buyer has approved the subscription.
        /// </summary>
        APPROVED,

        /// <summary>
        /// The subscription is active.
        /// </summary>
        ACTIVE,

        /// <summary>
        /// The subscription is suspended.
        /// </summary>
        SUSPENDED,

        /// <summary>
        /// The subscription is cancelled.
        /// </summary>
        CANCELLED,

        /// <summary>
        /// The subscription is expired.
        /// </summary>
        EXPIRED
    }
}
