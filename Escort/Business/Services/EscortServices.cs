using Business.Communication;
using Business.IServices;
using Data.IRepository;
using Data.Repository;
using DocumentFormat.OpenXml.Spreadsheet;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Extensions;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Escort;
using Shared.Model.Request.Admin;
using Shared.Model.Request.Profile;
using Shared.Model.Request.WebUser;
using Shared.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class EscortServices : IEscortServices
    {
        private readonly IEscortRepository _escortRepository;
        private readonly INotificationService _notificationService;
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IAccountRepository _accountRepository;
        public EscortServices(IEscortRepository escortRepository, INotificationService notificationService, IActivityLogRepository activityLogRepository, IAccountRepository accountRepository)
        {
            _escortRepository = escortRepository;
            _notificationService = notificationService;
            _activityLogRepository = activityLogRepository;
            _accountRepository = accountRepository;
        }

        public async Task<ApiResponse<List<EscortModel>>> EscortList(UsersRequestModel request)
        {
            ApiResponse<List<EscortModel>> response = new();
            var escortlist = await _escortRepository.EscortList(request);

            if (escortlist is null)
            {
                response.Data = new List<EscortModel>();
                response.Message = ResourceString.Error;
                return response;
            }

            escortlist.ForEach(item =>
            {
                item.Category = item.UserType == (short)UserTypes.IndependentEscort ? "Independent" : "Establishment";
            });

            response.Data = escortlist;
            response.Message = ResourceString.Success;
            return response;
        }

        public async Task<int> ChangeEscortStatus(int userId, bool activeStatus, bool deleteStatus)
        {
            UserDetail requestModel = new();
            requestModel.Id = userId;
            requestModel.IsActive = activeStatus;
            requestModel.IsDeleted = deleteStatus;
            requestModel.AccessToken = Guid.NewGuid().ToString();

            var result = await _escortRepository.AddUpdateAsync(requestModel);
            if (result > 0)
            {
                await SaveActivityLog(userId, activeStatus ? ActivityType.ActivateProfile : ActivityType.DeActivateProfile);
            }
            return result;
        }

        public async Task<List<EscortSearchDto>> GetEscortsByIds(List<RequestId> ids)
        {
            return await _escortRepository.GetEscortsByIds(ids);
        }

        public Task SendContactUsMailToEscort(string emailsubject, EmailRequestModel model)
        {
            return _notificationService.SendContactUsMailToEscort(emailsubject, model);
        }

        public async Task<List<CountryWiseEscortDto>> GetCountryWiseEscortCount()
        {
            return await _escortRepository.GetCountryWiseEscortCount();
        }

        public async Task<List<GetLastTwelveMonthGiftTokensDto>> GetLastTwelveMonthGiftTokens()
        {
            return await _escortRepository.GetLastTwelveMonthGiftTokens();
        }

        public async Task<List<GetLastTwelveMonthSubscriptionReportDto>> GetLastTwelveMonthSubscriptionReport()
        {
            return await _escortRepository.GetLastTwelveMonthSubscriptionReport();
        }

        public async Task<List<GetLastTwelveMonthRevenueReportDto>> GetLastTwelveMonthRevenueReport(decimal adminPercentage)
        {
            return await _escortRepository.GetLastTwelveMonthRevenueReport(adminPercentage);
        }

        public async Task<AdminDashboardStatisticsDto> GetAdminDashboardStatistics(decimal adminPercentage)
        {
            return await _escortRepository.AdminDashboardStatistics(adminPercentage);
        }

        public async Task<int> GetEscortScore(decimal userId)
        {
            return await _escortRepository.GetEscortScore(userId);
        }

        public async Task<int> StartLiveCam(int escortUserId)
        {
            return await _escortRepository.StartLiveCam(escortUserId);
        }

        public async Task<int> StopLiveCam(int escortUserId)
        {
            return await _escortRepository.StopLiveCam(escortUserId);
        }

        public async Task<int> JoinLiveCam(int clientId, int escortId)
        {
            return await _escortRepository.JoinLiveCam(clientId, escortId);
        }

        public async Task<GetEscortDashboardStatisticsDto> GetEscortDashboardStatistics(int escortUserId, decimal adminPercentage)
        {
            return await _escortRepository.GetEscortDashboardStatistics(escortUserId, adminPercentage);
        }

        public async Task<ApiResponse<List<EscortLiveCamGiftSummeryDto>>> EscortLiveCamGiftSummery(EscortLiveCamGiftSummeryRequest request, decimal adminPercentage)
        {
            ApiResponse<List<EscortLiveCamGiftSummeryDto>> response = new();
            var escortGiftSummery = await _escortRepository.GetEscortLiveCamGiftSummery(request, adminPercentage);

            if (escortGiftSummery is null)
            {
                response.Data = new List<EscortLiveCamGiftSummeryDto>();
                response.Message = ResourceString.Error;
                return response;
            }

            response.Data = escortGiftSummery;
            response.Message = ResourceString.Success;
            return response;
        }

        #region ActivityLog

        private async Task SaveActivityLog(int targetUserId, ActivityType activityType)
        {
            var targetUserDetail = await _accountRepository.GetByIdAsync(targetUserId);
            var activityRequest = GetActivityLogRequestModel(targetUserDetail, activityType, 0);
            var activitySaveModel = CommonFunctions.GetActivityLogModel(activityRequest);
            if (activitySaveModel != null)
            {
                await _activityLogRepository.AddUpdateAsync(activitySaveModel);
            }
        }

        private ActivityLogRequestModel GetActivityLogRequestModel(UserDetail targetUserDetail, ActivityType activityType, decimal amount)
        {
            return new ActivityLogRequestModel()
            {
                TargetUser = new ActivityUser()
                {
                    UserId = targetUserDetail.Id,
                    UserName = $"{targetUserDetail.FirstName} {targetUserDetail.LastName}",
                    UserRole = EnumExtensions.GetDescription((UserTypes)targetUserDetail.UserType)
                },
                LoggedInUser = new ActivityUser()
                {
                    UserId = LoginMemberSession.UserDetailSession?.UserId ?? 0,
                    UserName = $"{LoginMemberSession.UserDetailSession?.FirstName ?? ""} {LoginMemberSession.UserDetailSession?.LastName ?? ""}",
                    UserRole = LoginMemberSession.UserDetailSession == null ? "Anonymous" :  EnumExtensions.GetDescription((UserTypes)LoginMemberSession.UserDetailSession.UserTypeId)
                },
                ActivityType = activityType,
                Amount = amount,
                DbEntity = DbEntityType.UserDetail,
                TargetId = targetUserDetail.Id,
            };
        }

        #endregion ActivityLog
    }
}
