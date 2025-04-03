using Shared.Common.Enums;

namespace Shared.Common
{
    public class EscortRules
    {
        public int AccountDuration { get; set; }
        public bool VipEscort { get; set; }
        public int CanContainEscorts { get; set; }
        public int AllowedBaseLocations { get; set; }
        public int AllowedTourLocations { get; set; }
        public int AllowedPhotoGallery { get; set; }
        public int AllowedVideos { get; set; }
        public PhotoUploadLimit PhotoUploadLimit { get; set; } = PhotoUploadLimit.UnLimited;
        public bool PhotoVerification { get; set; } = true;
        public bool PhotoBlurring { get; set; } = true;
        public bool ConnectToSocialMedia { get; set; } = false;

        public EscortRules(UserTypes? userType, SubscriptionPlanDurationType? subscriptionPlanId)
        {
            if (userType == UserTypes.IndependentEscort)
            {
                switch (subscriptionPlanId)
                {
                    case SubscriptionPlanDurationType.IndependentEscortBasic:
                        Initialize(0, 1, 6, 0, 0, 0, false, true);
                        break;
                    case SubscriptionPlanDurationType.IndependentEscortWeeklyStarter:
                        Initialize(1, 2, 10, 0, 1, 0, false, true);
                        break;
                    case SubscriptionPlanDurationType.IndependentEscortMonthly:
                        Initialize(4, 5, 15, 0, 2, 0, false, true);
                        break;
                    case SubscriptionPlanDurationType.IndependentEscortQuarterly:
                        Initialize(12, 10, 30, 5, 5, 0, true, true); 
                        break;
                    
                    case SubscriptionPlanDurationType.IndependentEscortYearly:
                        Initialize(52, 10, 30, 5, 5, 0, true, true);
                        break;
                    default:
                        Initialize(0, 0, 0, 0, 0, 0, false, false);
                        break;
                }
            }
            else if (userType == UserTypes.Establishment)
            {
                switch (subscriptionPlanId)
                {
                    case SubscriptionPlanDurationType.EstablishmentWeeklyStarter:
                        Initialize(1, 5, 15, 0, 1, 12, false, false);
                        break;
                    case SubscriptionPlanDurationType.EstablishmentWeeklyPro:
                        Initialize(1, 8, 20, 0, 3, 18, false, false);
                        break;
                    case SubscriptionPlanDurationType.EstablishmentMonthly:
                        Initialize(4, 10, 30, 5, 5, 24, true, false);
                        break;
                    case SubscriptionPlanDurationType.EstablishmentQuarterMonthly:
                        Initialize(12, 10, 30, 5, 5, 24, true, false);
                        break;
                    default:
                        Initialize(0, 0, 0, 0, 0, 0, false, false);
                        break;
                }
            }
            else
            {
                Initialize(0, 0, 0, 0, 0, 0, false, false);
            }
        }

        private void Initialize(int accountDuration, int allowedBaseLocations, int allowedPhotoGallery, int allowedTourLocations, int allowedVideos, int canContainEscorts, bool vipEscort, bool connectToSocialMedia)
        {
            AccountDuration = accountDuration;
            AllowedBaseLocations = allowedBaseLocations;
            AllowedPhotoGallery = allowedPhotoGallery;
            AllowedTourLocations = allowedTourLocations;
            AllowedVideos = allowedVideos;
            CanContainEscorts = canContainEscorts;
            VipEscort = vipEscort;
            ConnectToSocialMedia = connectToSocialMedia;
        }
    }

    public enum PhotoUploadLimit
    {
        UnLimited
    }
}
