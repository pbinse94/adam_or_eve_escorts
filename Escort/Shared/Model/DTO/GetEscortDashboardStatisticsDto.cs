namespace Shared.Model.DTO
{
    public class GetEscortDashboardStatisticsDto
    {
        public decimal TotalRevenueFromGifts { get; set; }
        public int TotalLiveCam { get; set; }
        public int TotalCreditsReceived { get; set; }
        public int TotalRegisteredUsersOnCams { get; set; }
        public int TotalLiveCamsDuration { get; set; }
    }
}
