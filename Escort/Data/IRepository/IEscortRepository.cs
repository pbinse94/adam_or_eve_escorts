using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Escort;
using Shared.Model.Request.Admin;
using Shared.Model.Request.WebUser;

namespace Data.IRepository
{
    public interface IEscortRepository : ICurdRepository<UserDetail>
    {
        Task<List<EscortModel>> EscortList(UsersRequestModel request);
        Task<List<EscortSearchDto>> GetEscortsByIds(List<RequestId> ids);
        Task<List<CountryWiseEscortDto>> GetCountryWiseEscortCount();
        Task<List<GetLastTwelveMonthGiftTokensDto>> GetLastTwelveMonthGiftTokens();
        Task<List<GetLastTwelveMonthSubscriptionReportDto>> GetLastTwelveMonthSubscriptionReport();
        Task<List<GetLastTwelveMonthRevenueReportDto>> GetLastTwelveMonthRevenueReport(decimal adminPercentage);
        Task<AdminDashboardStatisticsDto> AdminDashboardStatistics(decimal adminPercentage);
        Task<int> GetEscortScore(decimal userId);
        Task<int> StartLiveCam(int escortUserId);
        Task<int> StopLiveCam(int escortUserId);
        Task<int> JoinLiveCam(int clientId, int escortId);
        Task<GetEscortDashboardStatisticsDto> GetEscortDashboardStatistics(int escortUserId, decimal adminPercentage);
        Task<List<EscortLiveCamGiftSummeryDto>> GetEscortLiveCamGiftSummery(EscortLiveCamGiftSummeryRequest request, decimal adminPercentage);
    }
}
