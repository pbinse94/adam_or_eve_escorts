using Azure;
using Azure.Core;
using Business.Communication;
using Business.IServices;
using Data.IRepository;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Extensions;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Account;
using Shared.Model.Request.Admin;
using Shared.Model.Request.AdminUser;
using Shared.Resources;
using Shared.Utility;
using System;
using System.Reflection;

namespace Business.Services
{
    public class ManageService : IManageService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationService _notificationService;
        private readonly IActivityLogRepository _activityLogRepository;

        public ManageService(INotificationService notificationService, IAccountRepository accountRepository, IFileStorageService fileStorageService, IHttpContextAccessor httpContextAccessor, IActivityLogRepository activityLogRepository)
        {
            _accountRepository = accountRepository;
            _fileStorageService = fileStorageService;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _activityLogRepository = activityLogRepository;
        }
        public async Task<ApiResponse<List<UsersDto>>> UserList(UsersRequestModel request)
        {
            ApiResponse<List<UsersDto>> response = new();
            var getUserList = await _accountRepository.UserList(request);

            if (getUserList is null)
            {
                response.Data = new List<UsersDto>();
                response.Message = ResourceString.Error;
                return response;
            }

            getUserList.ForEach(u =>
            {
                u.PhoneNumber = u.PhoneNumber ?? "N/A";
                u.RegisterDate = u.CreatedOn.ToShortDateString();
            });

            response.Data = getUserList;
            response.Message = ResourceString.Success;

            return response;
        }

        public async Task<ApiResponse<UserDetailsDto>> GetUserDetails(int userId)
        {
            ApiResponse<UserDetailsDto> response = new();
            UserDetailsDto userDetailsObj = new();
            var getUserDetail = await _accountRepository.GetByIdAsync(userId);

            if (getUserDetail is null)
            {
                response.Data = userDetailsObj;
                response.Message = ResourceString.Fail;
                return response;
            }

            userDetailsObj.UserId = getUserDetail.Id;
            userDetailsObj.UserType = getUserDetail.UserType;
            userDetailsObj.FirstName = getUserDetail.FirstName ?? "N/A";
            userDetailsObj.LastName = getUserDetail.LastName ?? "N/A";
            userDetailsObj.DisplayName = getUserDetail.DisplayName ?? "N/A";
            userDetailsObj.PhoneNumber = getUserDetail.PhoneNumber;
            userDetailsObj.Email = getUserDetail.Email ?? "N/A";
            userDetailsObj.IsActive = getUserDetail.IsActive;
            userDetailsObj.IsDeleted = getUserDetail.IsDeleted;
            userDetailsObj.ProfileImage = getUserDetail.ProfileImage;
            userDetailsObj.Country = getUserDetail.Country;
            userDetailsObj.CountryCode = getUserDetail.CountryCode;
            userDetailsObj.CreditBalance = getUserDetail.TotalToken;


            response.Data = userDetailsObj;
            response.Message = ResourceString.Success;
            return response;
        }

        public async Task<ApiResponse<UserDetailsDto>> UpdateUserDetail(UserDetailsDto requestModel)
        {
            ApiResponse<UserDetailsDto> response = new();


            string newImageName = string.Empty;
            if (requestModel.CroppedProfileFile != null)
            {
                if (requestModel.CroppedProfileFile != "")
                {
                    newImageName = await _fileStorageService.UploadFileByBase64(requestModel.CroppedProfileFile, Constants.ProfileImage, "images");
                }

            }
            var getUserDetail = await _accountRepository.GetByIdAsync(requestModel.UserId);

            if (!string.IsNullOrEmpty(newImageName))
            {
                requestModel.ProfileImage = newImageName;
            }
            else
            {
                requestModel.ProfileImage = getUserDetail.ProfileImage;
            }
            var updateUser = await _accountRepository.AddUpdateAsync(new UserDetail
            {
                Id = requestModel.UserId,
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName,
                DisplayName = requestModel.DisplayName,
                PhoneNumber = requestModel.PhoneNumber,
                ProfileImage = requestModel.ProfileImage,
                Country = requestModel.Country,
                CountryCode = requestModel.CountryCode
            });

            if (updateUser > 0)
            {
                response.Message = ResourceString.ProfileUpdateSuccess;
                response.Data = requestModel;
            }
            else
            {
                response.Message = ResourceString.ProfileUpdateFailed;
                response.Data = new UserDetailsDto();
            }
            return response;
        }

        public async Task<ResponseTypes> ChangePassword(ChangePasswordModel model, int userId)
        {
            var getUserDetail = await _accountRepository.GetByIdAsync(userId);

            if (getUserDetail is null)
            {
                return ResponseTypes.Error;
            }

            if (!Encryption.VerifyHash(model.OldPassword, getUserDetail.PasswordHash))
            {
                return ResponseTypes.OldPasswordWrong;
            }
            else
            {
                if (Encryption.VerifyHash(model.Password, getUserDetail.PasswordHash))
                {
                    return ResponseTypes.OldNewPasswordMatched;
                }
                else
                {
                    model.Password = Encryption.ComputeHash(model.Password);
                    var updatePassword = await _accountRepository.ChangePassword(model, userId);
                    if (updatePassword > 0)
                    {
                        return ResponseTypes.Success;
                    }
                    else
                    {
                        return ResponseTypes.Error;
                    }
                }

            }
        }
        public async Task<int> ChangeUserStatus(int userId, bool activeStatus, bool deleteStatus)
        {
            UserDetail requestModel = new();
            requestModel.Id = userId;
            requestModel.IsActive = activeStatus;
            requestModel.IsDeleted = deleteStatus;
            requestModel.AccessToken = Guid.NewGuid().ToString();

            var result = await _accountRepository.AddUpdateAsync(requestModel);
            if (result > 0)
            {
                await SaveActivityLog(userId, activeStatus ? ActivityType.ActivateProfile : ActivityType.DeActivateProfile);
            }
            return result;
        }
        public async Task<int> AddTestimonials(int escortId, string testimonials, int userId)
        {
            return await _accountRepository.AddTestimonials(escortId, testimonials, userId);
        }
        public async Task<List<TestimonialModel>> GetTestimonialAsync(int escortId)
        {
            var result = await _accountRepository.GetTestimonialAsync(escortId);
            foreach (var model in result)
            {
                model.AddedOnUTC = Convert.ToDateTime(DateTimeExtensions.ToLocal(model.AddedOnUTC, _httpContextAccessor));
            }
            return result;
        }



        // admin users

        public async Task<ApiResponse<List<UsersDto>>> AdminUserList(UsersRequestModel request)
        {
            ApiResponse<List<UsersDto>> response = new();
            var getUserList = await _accountRepository.AdminUserList(request);

            if (getUserList is null)
            {
                response.Data = new List<UsersDto>();
                response.Message = ResourceString.Error;
                return response;
            }

            response.Data = getUserList;
            response.Message = ResourceString.Success;

            return response;
        }

        public async Task<ApiResponse<List<ModulePermissionModel>>> GetUserPermission(int userId)
        {
            ApiResponse<List<ModulePermissionModel>> response = new();
            var getUserList = await _accountRepository.GetUserPermission(userId);

            var userPermissions = getUserList
                .GroupBy(x => new { x.ModuleId, x.ModuleName, x.CanAdd, x.CanEdit, x.CanView, x.CanDelete })
                .Select(g => new ModulePermissionModel
                {
                    ModuleId = g.Key.ModuleId,
                    ModuleName = g.Key.ModuleName,

                    CanAdd = g.Key.CanAdd,
                    CanEdit = g.Key.CanEdit,
                    CanView = g.Key.CanView,
                    CanDelete = g.Key.CanDelete,

                    UserId = userId,
                    PermissionModel = g.Select(p => new PermissionModel
                    {
                        PermissionId = p.PermissionId,
                        PermissionName = p.PermissionName,
                        ModuleId = p.ModuleId,
                        IsOn = p.IsOn
                    }).ToList()
                }).ToList();

            if (getUserList is null)
            {
                response.Data = new List<ModulePermissionModel>();
                response.Message = ResourceString.Error;
                return response;
            }

            response.Data = userPermissions;
            response.Message = ResourceString.Success;

            return response;
        }        

        public async Task<ApiResponse<bool>> AddUpdateAdminUser(AdminUserRequestModel requestModel, int loogedInUserId)
        {
            ApiResponse<bool> response = new();


            List<AdminUserPermissionRequestModel> permissionList = requestModel.ModulePermissions
                .SelectMany(module => module.PermissionModel.Select(permission => new AdminUserPermissionRequestModel
                {
                    UserId = requestModel.Id,
                    ModuleId = module.ModuleId,
                    PermissionId = permission.PermissionId,
                    IsOn = permission.IsOn
                })).ToList();

            string password = string.Empty;
            if (requestModel.Id <= 0)
            {
                password = CommonFunctions.GetAlphanumericRandomPassword(6);
                requestModel.PasswordHash = Encryption.ComputeHash(password);
                requestModel.EmailVerifiedToken = Guid.NewGuid().ToString();
            }

            var res = await _accountRepository.AddUpdateAdminUser(requestModel, permissionList, loogedInUserId);

            if (res.Status > 0)
            {
                if (res.Status == 1)
                {
                    await SaveActivityLog(res.UserId, requestModel.Id ==0 ? ActivityType.CreateProfile:ActivityType.UpdateProfile);
                    response.Data = true;
                    response.Message = ResourceString.StaffUpdated;
                }
                else
                {
                    response.Data = true;
                    response.Message = ResourceString.StaffAdded;

                    // send email verification mail to staff users
                    await _notificationService.StaffEmailVerification(requestModel.Email, requestModel.DisplayName, requestModel.EmailVerifiedToken ?? string.Empty, ResourceString.VerifyEmailSubject, password);

                }
            }
            else
            {
                if (res.Status == -1)
                {
                    response.Data = false;
                    response.Message = ResourceString.UserExists;
                }
                else
                {
                    response.Data = false;
                    response.Message = ResourceString.Error;
                }
            }

            return response;
        }

        public async Task<ApiResponse<bool>> IsUserEmailInUse(string email, int id)
        {
            var res = await _accountRepository.IsUserEmailInUse(email, id);
            ApiResponse<bool> response = new();
            if (res > 0)
            {
                response.Data = true;
                response.Message = ResourceString.UserExists;
                return response;
            }


            response.Data = false;
            response.Message = ResourceString.Success;

            return response;
        }

        public async Task<ApiResponse<List<LoginHistoryDto>>> UserLoginHistory(LoginHistoryRequestModel request)
        {
            ApiResponse<List<LoginHistoryDto>> response = new();
            var loginHistory = await _accountRepository.UserLoginHistory(request);

            if (loginHistory is null)
            {
                response.Data = new List<LoginHistoryDto>();
                response.Message = ResourceString.Error;
                return response;
            }

            loginHistory.ForEach(u =>
            {
                u.LoginDateString = u.LoginTime is null ? "N/A" : u.LoginTime.Value.ToLocal(_httpContextAccessor);
                u.LogOutDateString = u.LogOutTime is null ? "N/A" : u.LogOutTime.Value.ToLocal(_httpContextAccessor);

            });

            response.Data = loginHistory;
            response.Message = ResourceString.Success;
            return response;

        }

        public async Task<ApiResponse<List<ActivityLogDto>>> GetAdminUserActivities(ActivityLogHistoryRequestModel request)
        {
            ApiResponse<List<ActivityLogDto>> response = new();
            var activityLogHistory = await _activityLogRepository.ActivityLogHistory(request);

            if (activityLogHistory is null)
            {
                response.Data = new List<ActivityLogDto>();
                response.Message = ResourceString.Error;
                return response;
            }

            activityLogHistory.ForEach(u =>
            {
                u.ActionDateString = u.ActionDate is null ? "N/A" : u.ActionDate.Value.ToLocal(_httpContextAccessor);

            });

            response.Data = activityLogHistory;
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
