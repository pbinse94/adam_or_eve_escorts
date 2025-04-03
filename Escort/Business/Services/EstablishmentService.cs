using Business.IServices;
using Data.IRepository;
using Data.Repository;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Http;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Extensions;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Escort;
using Shared.Model.Establishment;
using Shared.Model.Request.Admin;
using Shared.Model.Request.WebUser;
using Shared.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class EstablishmentService : IEstablishmentService
    {
        private readonly IEstablishmentRepository _establishmentRepository;
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EstablishmentService(IEstablishmentRepository establishmentRepository, IActivityLogRepository activityLogRepository, IAccountRepository accountRepository, ISubscriptionPlanRepository subscriptionPlanRepository, IHttpContextAccessor httpContextAccessor)
        {
            _establishmentRepository = establishmentRepository;
            _activityLogRepository = activityLogRepository;
            _accountRepository = accountRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<List<EstablishmentModel>>> EstablishmentList(UsersRequestModel request)
        {
            ApiResponse<List<EstablishmentModel>> response = new();
            var escortlist = await _establishmentRepository.GetEstablishments(request);

            if (escortlist is null)
            {
                response.Data = new List<EstablishmentModel>();
                response.Message = ResourceString.Error;
                return response;
            }

            response.Data = escortlist;
            response.Message = ResourceString.Success;
            return response;
        }

        public async Task<int> ChangeEstablishmentStatus(int userId, bool activeStatus)
        {
            var result = await _establishmentRepository.ChangeEstablishmentStatus(userId, activeStatus);
            if (result > 0)
            {
                await SaveActivityLog(userId, activeStatus ? ActivityType.ActivateProfile : ActivityType.DeActivateProfile);
            }
            return result;
        }

        public async Task<ApiResponse<EscortDetailDto>> GetEstablishmentById(int userId)
        {
            var getUserDetail = await _accountRepository.GetByIdAsync(userId);

            if (getUserDetail is null)
            {
                return new ApiResponse<EscortDetailDto>(null, message: ResourceString.UserDetailsNotFound, apiName: "GetEscortProfileDetails");
            }

            EscortDetailDto userDetailsDto = new EscortDetailDto
            {
                //-- User details
                UserId = userId,
                FirstName = getUserDetail.FirstName,
                LastName = getUserDetail.LastName,
                Email = getUserDetail.Email,
                ProfileImage = getUserDetail.ProfileImage,
                PhoneNumber = getUserDetail.PhoneNumber,
                DisplayName = getUserDetail.DisplayName,
                UserType = Convert.ToInt16(getUserDetail.UserType),

                UpdatedOnUTC = getUserDetail.UpdatedOnUTC,
                //-- Escorts details

                Gender = getUserDetail.Gender,
                Country = getUserDetail.Country,
                CountryCode = getUserDetail.CountryCode
            };

            var activeSubscription = await _subscriptionPlanRepository.GetUserSubscriptionDetail(userId);

            if (activeSubscription != null)
            {

                userDetailsDto.EscortSubscription = new EscortSubscriptionDto()
                {
                    PurchaseDateString = activeSubscription.PurchaseDateUTC.ToLocal(_httpContextAccessor),
                    ExpiryDateString = activeSubscription.ExpiryDateUTC.ToLocal(_httpContextAccessor)
                };
                
            }
            return new ApiResponse<EscortDetailDto>(userDetailsDto, message: ResourceString.GetUserDetails, apiName: "GetEstablishmentById");
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
                    UserRole = $"{Enum.GetName(typeof(UserTypes), targetUserDetail.UserType)}"
                },
                LoggedInUser = new ActivityUser()
                {
                    UserId = LoginMemberSession.UserDetailSession?.UserId ?? 0,
                    UserName = $"{LoginMemberSession.UserDetailSession?.FirstName ?? ""} {LoginMemberSession.UserDetailSession?.LastName ?? ""}",
                    UserRole = LoginMemberSession.UserDetailSession == null ? "Anonymous" : EnumExtensions.GetDescription((UserTypes)LoginMemberSession.UserDetailSession.UserTypeId)
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
