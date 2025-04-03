using Business.IServices;
using Data.IRepository;
using Data.Repository;
using DocumentFormat.OpenXml.Spreadsheet;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Admin;
using Shared.Model.Request.WebUser;
using Shared.Resources;

namespace Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IActivityLogRepository _activityLogRepository;
        public UserService(IUserRepository userRepository, IAccountRepository accountRepository, IActivityLogRepository activityLogRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _activityLogRepository = activityLogRepository;
        }

        public async Task<ApiResponse<List<UserTokenTransactionDto>>> UserList(UserTokenTransactionRequestModel request)
        {
            ApiResponse<List<UserTokenTransactionDto>> response = new();
            var getUserList = await _userRepository.UserList(request);

            if (getUserList is null)
            {
                response.Data = new List<UserTokenTransactionDto>();
                response.Message = ResourceString.Error;
                return response;
            }

            response.Data = getUserList;
            response.Message = ResourceString.Success;

            return response;
        }



        public async Task<ApiResponse<List<UserTokenTransactionDto>>> ListByName(UserTokenTransactionRequestModel request)
        {
            ApiResponse<List<UserTokenTransactionDto>> response = new();
            var getUserList = await _userRepository.ListByName(request);

            if (getUserList is null)
            {
                response.Data = new List<UserTokenTransactionDto>();
                response.Message = ResourceString.Error;
                return response;
            }
            else
            {
                foreach (var item in getUserList)
                {
                    string transactionType = $"{Enum.GetName(typeof(GiftTransactionType), item.TransactionType)}";
                    item.Description = transactionType;
                }
            }
            response.Data = getUserList;
            response.Message = ResourceString.Success;

            return response;
        }

        public async Task<ApiResponse<GetClientBalanceReportDto>> GetClientBalanceReport(UserTokenTransactionRequestModel request, int loginUserId)
        {
            ApiResponse<GetClientBalanceReportDto> response = new();
            var getClientTransaction = await _userRepository.GetClientBalanceReport(request, loginUserId);

            if (getClientTransaction is null)
            {
                response.Data = null;
                response.Message = ResourceString.Error;
                return response;
            }

            await SaveActivityLog(request.UserId, ActivityType.ExportClientBalanceReport);

            response.Data = getClientTransaction;
            response.Message = ResourceString.Success;

            return response;
        }

        public async Task<int> SendGift(SendGiftRequest request)
        {
            return await _userRepository.SendGift(request);
        }

        public async Task<int> CheckAndInsertIPAddress(string ipAddress)
        {
            return await _userRepository.CheckAndInsertIPAddress(ipAddress);
        }


        public async Task<ApiResponse<List<PaymentReportDto>>> PaymentReportList(PaymentReportRequestModel request)
        {
            ApiResponse<List<PaymentReportDto>> response = new();
            var getUserList = await _userRepository.PaymentReportList(request);

            if (getUserList is null)
            {
                response.Data = new List<PaymentReportDto>();
                response.Message = ResourceString.Error;
                return response;
            }

            response.Data = getUserList;
            response.Message = ResourceString.Success;

            return response;
        }

        public async Task<ApiResponse<List<PaymentReportDto>>> ExportPaymentReportList(PaymentReportRequestModel request)
        {
            ApiResponse<List<PaymentReportDto>> response = new();
            var getUserList = await _userRepository.PaymentReportList(request);

            if (getUserList is null)
            {
                response.Data = new List<PaymentReportDto>();
                response.Message = ResourceString.Error;
                return response;
            }

            if (getUserList.Count > 0)
            {
                await SaveActivityLog(0, ActivityType.ExportPaymentReport);
            }

            response.Data = getUserList;
            response.Message = ResourceString.Success;

            return response;
        }


        public async Task<ApiResponse<List<UserTokenTransactionClientDto>>> GetClientBalanceReportAdmin(UserTokenTransactionRequestModel request) 
        {
            ApiResponse<List<UserTokenTransactionClientDto>> response = new();
            var getClientBalanceReportAdminList = await _userRepository.GetClientBalanceReportAdmin(request);

            if (getClientBalanceReportAdminList is null)
            {
                response.Data = new List<UserTokenTransactionClientDto>();
                response.Message = ResourceString.Error;
                return response;
            }

            response.Data = getClientBalanceReportAdminList;
            response.Message = ResourceString.Success;

            return response;
        }

       

        public async Task<ApiResponse<List<EscortPaymentAmountFromClientDto>>> EscortPaymentReportList(EscortPaymentReportRequestModel request)
        {
            ApiResponse<List<EscortPaymentAmountFromClientDto>> response = new();
            var getUserList = await _userRepository.EscortPaymentReportList(request);

            if (getUserList is null)
            {
                response.Data = new List<EscortPaymentAmountFromClientDto>();
                response.Message = ResourceString.Error;
                return response;
            }

            response.Data = getUserList;
            response.Message = ResourceString.Success;

            return response;
        }

        //Task<int> MarkPaymentDone(string usersid)


        public async Task<ApiResponse<int>> MarkPaymentDone(string usersId)
        {
            ApiResponse<int> response = new();
            var getUserList = await _userRepository.MarkPaymentDone(usersId);

            if (getUserList == 0)
            {
                response.Data = getUserList;
                response.Message = ResourceString.Error;
                return response;
            }

            foreach (var user in usersId.Split(",")) 
            {
                await SaveActivityLog(Convert.ToInt32(user), ActivityType.MarkPayment);
            }

            response.Data = getUserList;
            response.Message = ResourceString.Success;

            return response;
        }

        #region ActivityLog

        private async Task SaveActivityLog(int targetUserId, ActivityType activityType)
        {
            UserDetail? targetUserDetail = null;
            if(targetUserId > 0)
            {
                targetUserDetail = await _accountRepository.GetByIdAsync(targetUserId);                
            }

            var activityRequest = GetActivityLogRequestModel(targetUserDetail, activityType, 0);
            var activitySaveModel = CommonFunctions.GetActivityLogModel(activityRequest);
            if (activitySaveModel != null)
            {
                await _activityLogRepository.AddUpdateAsync(activitySaveModel);
            }
        }

        private ActivityLogRequestModel GetActivityLogRequestModel(UserDetail? targetUserDetail, ActivityType activityType, decimal amount)
        {
            return new ActivityLogRequestModel()
            {
                TargetUser = targetUserDetail is null ? null : new ActivityUser()
                {
                    UserId = targetUserDetail.Id,
                    UserName = $"{targetUserDetail.FirstName} {targetUserDetail.LastName}",
                    UserRole = $"{Enum.GetName(typeof(UserTypes), targetUserDetail.UserType)}"
                },
                LoggedInUser = new ActivityUser()
                {
                    UserId = LoginMemberSession.UserDetailSession?.UserId ?? 0,
                    UserName = $"{LoginMemberSession.UserDetailSession?.FirstName ?? ""} {LoginMemberSession.UserDetailSession?.LastName ?? ""}",
                    UserRole = $"{Enum.GetName(typeof(UserTypes), LoginMemberSession.UserDetailSession?.UserTypeId ?? 0)}"
                },
                ActivityType = activityType,
                Amount = amount,
                DbEntity = DbEntityType.UserDetail,
                TargetId = targetUserDetail?.Id ?? 0,
            };
        }

        #endregion ActivityLog
    }
}