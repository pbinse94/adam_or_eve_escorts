using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Escort;
using Shared.Model.Request.Admin;
using Shared.Model.Request.Profile;
using Shared.Model.Request.WebUser;

namespace Business.IServices
{
    public interface IEscortServices
    {
        Task<ApiResponse<List<EscortModel>>> EscortList(UsersRequestModel request);
        Task<int> ChangeEscortStatus(int userId, bool activeStatus, bool deleteStatus);
        Task<List<EscortSearchDto>> GetEscortsByIds(List<RequestId> ids);
        Task SendContactUsMailToEscort(string emailsubject, EmailRequestModel model);
        Task<List<CountryWiseEscortDto>> GetCountryWiseEscortCount();
        Task<List<GetLastTwelveMonthGiftTokensDto>> GetLastTwelveMonthGiftTokens();
        Task<List<GetLastTwelveMonthSubscriptionReportDto>> GetLastTwelveMonthSubscriptionReport();
        Task<List<GetLastTwelveMonthRevenueReportDto>> GetLastTwelveMonthRevenueReport(decimal adminPercentage);
        Task<AdminDashboardStatisticsDto> GetAdminDashboardStatistics(decimal adminPercentage);
        Task<int> GetEscortScore(decimal userId);
        Task<int> StartLiveCam(int escortUserId);
        Task<int> StopLiveCam(int escortUserId);
        Task<int> JoinLiveCam(int clientId, int escortId);
        Task<GetEscortDashboardStatisticsDto> GetEscortDashboardStatistics(int escortUserId, decimal adminPercentage);
        Task<ApiResponse<List<EscortLiveCamGiftSummeryDto>>> EscortLiveCamGiftSummery(EscortLiveCamGiftSummeryRequest request, decimal adminPercentage);
    }
}
