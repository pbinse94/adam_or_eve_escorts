using Business.Communication;
using Business.IServices;
using Data.IRepository;
using Data.Repository;
using DeviceId;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Common.Enums;
using Shared.Extensions;
using Shared.Model.Base;
using Shared.Model.DTO;
using Shared.Model.Entities;
using Shared.Model.Request.Account;
using Shared.Model.Request.Subscription;
using Shared.Model.Request.WebUser;
using Shared.Resources;
using Shared.Utility;
using System.Text;

namespace Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IEmailFunctions _emailFunctions;
        private readonly IActivityLogRepository _activityLogRepository;
        public AccountService(IAccountRepository accountRepository, INotificationService notificationService, ISubscriptionPlanRepository subscriptionPlanRepository, IEmailFunctions emailFunctions, IActivityLogRepository activityLogRepository)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _emailFunctions = emailFunctions;
            _activityLogRepository = activityLogRepository;
        }

        public async Task<List<GetCountryCodesDto>> GetCountryCodes()
        {
            return await _accountRepository.GetCountryCodes();
        }

        public async Task<ApiResponse<ProfileDto>> Login(ApiLoginRequest request)
        {

            var objUserContext = await _accountRepository.FindByEmailAsync(request.Email);
            List<short> allowedUserTypes = new List<short>() { (short)UserTypes.Client, (short)UserTypes.IndependentEscort, (short)UserTypes.Establishment };

            if (objUserContext == null)
            {
                return new ApiResponse<ProfileDto>(null, message: ResourceString.UserNotExist, apiName: "Login");
            }
            else if (request.UserType != null && objUserContext.UserType != (short)request.UserType)
            {
                return new ApiResponse<ProfileDto>(null, message: ResourceString.UserNotExist, apiName: "Login");
            }
            else if (!allowedUserTypes.Contains(objUserContext.UserType))
            {
                return new ApiResponse<ProfileDto>(null, message: ResourceString.UserNotExist, apiName: "Login");
            }
            else if (!objUserContext.IsEmailVerified)
            {
                return new ApiResponse<ProfileDto>(null, message: ResourceString.EmailNotVerified, apiName: "Login");
            }
            else if (!(objUserContext.IsActive ?? false))
            {
                return new ApiResponse<ProfileDto>(null, message: ResourceString.UserIsNotActive, apiName: "Login");
            }
            else if (!Encryption.VerifyHash(request.Password, objUserContext.PasswordHash))
            {
                return new ApiResponse<ProfileDto>(null, message: ResourceString.InvalidPassword, apiName: "Login");
            }

            var accessToken = Guid.NewGuid().ToString();


            var existingDeviceInfo = await _accountRepository.GetLoginDeviceInfo(objUserContext.UserId);
            var publicipAddress = GetPublicIpAddress().Result;
            await _accountRepository.ManageLoginAccessDetail(new ManageLoginAccessDetailRequest()
            {
                Email = request.Email,
                DeviceToken = request.DeviceToken,
                DeviceType = request.DeviceType,
                AccessToken = accessToken,
                IpAddress = publicipAddress,
                DeviceInfo = request.DeviceInfo,
            });



            if (existingDeviceInfo != null && !string.IsNullOrEmpty(existingDeviceInfo.DeviceToken) && existingDeviceInfo.DeviceToken != request.DeviceToken)
            {
                await _notificationService.LoginInAnotherDevice(objUserContext.Email, objUserContext.FirstName, accessToken, "Login Alert", request.DeviceInfo, request.DeviceLocation);
            }





            ProfileDto profileDto = new ProfileDto
            {
                UserId = objUserContext.UserId,
                FirstName = objUserContext.FirstName,
                LastName = objUserContext.LastName,
                DisplayName = objUserContext.DisplayName,
                Email = objUserContext.Email,
                ProfileImage = objUserContext.ProfileImage,
                AccessToken = accessToken,
                UserType = objUserContext.UserType,
                CreditBalance = objUserContext.CreditBalance
            };

            if (objUserContext.UserType == (short)UserTypes.Establishment || objUserContext.UserType == (short)UserTypes.IndependentEscort)
            {
                var activeSubscription = await _subscriptionPlanRepository.GetUserSubscriptionDetail(objUserContext.UserId);

                if (activeSubscription != null)
                {
                    profileDto.PlanType = activeSubscription.PlanType;
                    profileDto.PlanDuration = activeSubscription.PlanDuration;
                    profileDto.SubscriptionPlanId = activeSubscription.SubscriptionId;
                    profileDto.SubscriptionPlanExpireDateTime = activeSubscription.ExpiryDateUTC;
                    profileDto.SubscriptionPlanPaypalId = activeSubscription.SubscriptionPlanPaypalId;
                    profileDto.SubscriptionPaypalId = activeSubscription.PaymentSubscriptionId;

                }
            }


            return new ApiResponse<ProfileDto>(profileDto, message: ResourceString.Success, apiName: "Login");
        }


        public static async Task<string> GetPublicIpAddress()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Use a reliable external service to get your public IP
                    var response = await client.GetStringAsync("https://api.ipify.org");
                    return response.Trim();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, such as network errors
                Console.WriteLine($"Error getting public IP address: {ex.Message}");
                return "";
            }
        }

        public async Task<ApiResponse<ProfileDto>> AdminLogin(ApiLoginRequest request)
        {
            var objUserContext = await _accountRepository.FindByEmailAsync(request.Email);

            if (objUserContext == null)
            {
                return new ApiResponse<ProfileDto>(null, message: ResourceString.UserNotExist, apiName: "Login");
            }
            else if (!(new short[] { (short)UserTypes.SuperAdmin, (short)UserTypes.Admin, (short)UserTypes.Editor, (short)UserTypes.Viewer, (short)UserTypes.Management, (short)UserTypes.Accounting }
.Contains((short)objUserContext.UserType)))
            {
                return new ApiResponse<ProfileDto>(null, message: ResourceString.UserNotExist, apiName: "Login");
            }
            else if (!objUserContext.IsEmailVerified)
            {
                return new ApiResponse<ProfileDto>(null, message: ResourceString.EmailNotVerified, apiName: "Login");
            }
            else if (!(objUserContext.IsActive ?? false))
            {
                return new ApiResponse<ProfileDto>(null, message: ResourceString.UserIsNotActive, apiName: "Login");
            }
            else if (!Encryption.VerifyHash(request.Password, objUserContext.PasswordHash))
            {
                return new ApiResponse<ProfileDto>(null, message: ResourceString.InvalidPassword, apiName: "Login");
            }

            var accessToken = Guid.NewGuid().ToString();

            var existingDeviceInfo = await _accountRepository.GetLoginDeviceInfo(objUserContext.UserId);
            var publicipAddress = GetPublicIpAddress().Result;
            await _accountRepository.ManageLoginAccessDetail(new ManageLoginAccessDetailRequest()
            {
                Email = request.Email,
                DeviceToken = request.DeviceToken,
                DeviceType = request.DeviceType,
                AccessToken = accessToken,
                IpAddress = publicipAddress,
                DeviceInfo = request.DeviceInfo,
            });

            if (existingDeviceInfo != null && !string.IsNullOrEmpty(existingDeviceInfo.DeviceToken) && existingDeviceInfo.DeviceToken != request.DeviceToken)
            {
                await _notificationService.LoginInAnotherDevice(objUserContext.Email, objUserContext.FirstName, objUserContext.AccessToken, "Login Alert", request.DeviceInfo, request.DeviceLocation);
            }

            var activeSubscription = await _subscriptionPlanRepository.GetUserSubscriptionDetail(objUserContext.UserId);


            ProfileDto profileDto = new ProfileDto
            {
                UserId = objUserContext.UserId,
                FirstName = objUserContext.FirstName,
                LastName = objUserContext.LastName,
                DisplayName = objUserContext.DisplayName,
                Email = objUserContext.Email,
                ProfileImage = objUserContext.ProfileImage,
                AccessToken = accessToken,
                UserType = objUserContext.UserType
            };

            if (activeSubscription != null)
            {
                profileDto.PlanType = activeSubscription.PlanType;
                profileDto.PlanDuration = activeSubscription.PlanDuration;
            }


            return new ApiResponse<ProfileDto>(profileDto, message: ResourceString.Success, apiName: "Login");
        }

        public Task<UserDetailsDto> FindByEmailAsync(string email) => _accountRepository.FindByEmailAsync(email);

        public Task<UserDetailsDto> FindByEmailAndUpdateAsync(string email, string accessToken) => _accountRepository.FindByEmailAndUpdateAsync(email, accessToken);
        public static string GetAlphanumericRandomNumber(int digits)
        {
            Random rng = new Random();

            // Generate a 6-digit alphanumeric random number.
            var randomNumber = new StringBuilder(6);
            for (int i = 0; i < digits; i++)
            {
                // Get a random character from the alphanumeric character set.
                var randomChar = (char)(rng.Next(36) + 48);
                randomNumber.Append(randomChar);
            }
            return randomNumber.ToString();
        }




        public async Task<ApiResponse<ProfileDto>> SignUp(RegistrationRequest request)
        {
            var getUserDetail = await _accountRepository.FindByEmailAsync(request.Email);

            if (getUserDetail is not null)
            {
                return new ApiResponse<ProfileDto>(new ProfileDto(), message: ResourceString.EmailExists, apiName: "Signup");
            }
            if (string.IsNullOrEmpty(request.Password))
            {
                request.Password = CommonFunctions.GetAlphanumericRandomPassword(6);
            }
            var token = Guid.NewGuid().ToString();

            var userDetail = new UserDetail()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                Gender = request.Gender,
                Email = request.Email,
                PasswordHash = Encryption.ComputeHash(request.Password),
                UserType = (short)request.UserType,
                EmailVerifiedToken = token,
                IsEmailVerified = request.IsEmailVerified,
                IsActive = true,
                IsDeleted = false,
                DeviceType = (short)DeviceTypeEnum.Web,
                CountryCode = request.CountryCode,
                PhoneNumber = request.PhoneNumber,
                Country = request.Country,
                IsApprove = request.UserType != UserTypes.IndependentEscort
            };

            var newUser = await _accountRepository.AddUpdateScalerAsync(userDetail);

            if (newUser <= 0)
            {
                return new ApiResponse<ProfileDto>(new ProfileDto(), message: ResourceString.Error, apiName: "Signup");
            }
            if (request.UserType != UserTypes.EstablishmentEscort)
            {
                string message = request.UserType != UserTypes.Client ? ResourceString.WelcomeEmailMessage : "";
                await _notificationService.WelcomeEmail(request.Email, request.DisplayName, ResourceString.RegistrationSubject, message);
            }
            // login user if registeration successfully
            var accessToken = Guid.NewGuid().ToString();
            var existingDeviceInfo = await _accountRepository.GetLoginDeviceInfo(newUser);
            var publicipAddress = GetPublicIpAddress().Result;
            await _accountRepository.ManageLoginAccessDetail(new ManageLoginAccessDetailRequest()
            {
                Email = request.Email,
                DeviceToken = request.DeviceToken,
                DeviceType = (short)request.DeviceType,
                AccessToken = accessToken,
                IpAddress = publicipAddress,
                DeviceInfo = request.DeviceInfo,
            });
            if (existingDeviceInfo != null && !string.IsNullOrEmpty(existingDeviceInfo.DeviceToken) && existingDeviceInfo.DeviceToken != request.DeviceToken)
            {
                await _notificationService.LoginInAnotherDevice(request.Email, request.FirstName, accessToken, "Login Alert", request.DeviceInfo, request.DeviceLocation);
            }
            userDetail.Id = newUser;
            var activeSubscription = await _subscriptionPlanRepository.GetUserSubscriptionDetail(newUser);

            ProfileDto profileDto = new ProfileDto
            {
                UserId = newUser,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                Email = request.Email,
                // ProfileImage = request.ProfileImage,
                AccessToken = accessToken,
                UserType = (short)request.UserType
            };

            if (activeSubscription != null)
            {
                profileDto.PlanType = activeSubscription.PlanType;
                profileDto.PlanDuration = activeSubscription.PlanDuration;
            }

            ////await _notificationService.EmailVerification(request.Email, request.DisplayName, token, ResourceString.RegistrationSubject, Convert.ToInt32(request.DeviceType));

            return new ApiResponse<ProfileDto>(profileDto, message: ResourceString.SignUpSuccess, apiName: "Signup");
        }

        public async Task<ApiResponse<UserDetailsDto>> SaveFreeSubscription(string userEmail)
        {
            var userDetail = await _accountRepository.FindByEmailAsync(userEmail);

            SubscriptionPlan dbSubscription = await _subscriptionPlanRepository.GetFreeSubscriptionPlanDetail();

            UserSubscriptionRequest userSubscriptionDetail = new UserSubscriptionRequest()
            {
                UserId = userDetail.UserId,
                TransactionStatus = (byte)TransactionPaymentStatus.Success,
                SubscriptionId = (byte)dbSubscription.ID
            };

            var result = await _subscriptionPlanRepository.SaveUserSubscription(userSubscriptionDetail);
            if (result > 0)
            {
                userDetail.SubscriptionPlanDurationType = SubscriptionPlanDurationType.IndependentEscortBasic;
                string purchaseDate = System.DateTime.UtcNow.AddMinutes(SiteKeys.UtcOffset).ToString("dd MMM yy 'at' HH:mm") ?? string.Empty;
                string expiryDate = "";
                string subscriptionTypeString = $"{dbSubscription.Title} Plan";

                await _emailFunctions.PlanPurchasedSuccessMail(userDetail.Email, ResourceString.PlanPurchased, userDetail.DisplayName, subscriptionTypeString, purchaseDate, expiryDate);
                return new ApiResponse<UserDetailsDto>(data: userDetail);
            }
            else
            {
                return new ApiResponse<UserDetailsDto>(data: null);
            }
        }

        public async Task<ApiResponse<bool>> VerifyEmail(EmailVerifyRequest request)
        {
            var verifyEmail = await _accountRepository.EmailVerifyByToken(request.Token);
            if (!verifyEmail)
            {
                return new ApiResponse<bool>(false, message: ResourceString.InvalidConfirmationToken, apiName: "VerifyEmail");
            }
            return new ApiResponse<bool>(true, message: ResourceString.EmailVerified, apiName: "VerifyEmail");
        }

        public async Task<ApiResponse<bool>> ForgetPassword(ForgetPasswordRequest request)
        {
            var getUserByEmail = await _accountRepository.FindByEmailAsync(request.Email);
            if (getUserByEmail == null)
            {
                return new ApiResponse<bool>(false, message: ResourceString.UserNotExist, apiName: "ForgetPassword");
            }

            ForgetPasswordTokenRequest tokenRequest = new();
            tokenRequest.Email = request.Email;
            tokenRequest.Token = Guid.NewGuid().ToString();

            var updateUser = await _accountRepository.AddUpdateAsync(new UserDetail()
            {
                Email = tokenRequest.Email,
                ResetPasswordToken = tokenRequest.Token,
            });

            if (updateUser <= 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "ForgetPassword");
            }

            await _notificationService.SendResetPasswordEmail(ResourceString.ForgetPasswordSubject, tokenRequest.Token, tokenRequest.Email, getUserByEmail.DisplayName);
            return new ApiResponse<bool>(true, message: ResourceString.ForgetPassword, apiName: "ForgetPassword");
        }




        public async Task<ApiResponse<bool>> ResendVerificationLink(ResendVerificationLinkRequest request)
        {
            var getUserByEmail = await _accountRepository.FindByEmailAsync(request.Email);
            if (getUserByEmail == null)
            {
                return new ApiResponse<bool>(false, message: ResourceString.UserNotExist, apiName: "ResendVerificationLink");
            }
            int updateUser = 0;
            ResendVerificationLinkTokenRequest tokenRequest = new();

            if (string.IsNullOrEmpty(getUserByEmail.EmailVerificationToken))
            {
                tokenRequest.Email = request.Email;
                tokenRequest.Token = Guid.NewGuid().ToString();

                updateUser = await _accountRepository.AddUpdateAsync(new UserDetail()
                {
                    Email = tokenRequest.Email,
                    EmailVerifiedToken = tokenRequest.Token,
                    IsEmailVerified = false,
                    Id = getUserByEmail.UserId
                });

            }
            else
            {
                tokenRequest.Email = getUserByEmail.Email ?? "";
                tokenRequest.Token = getUserByEmail.EmailVerificationToken;
            }

            if (updateUser <= 0 && string.IsNullOrEmpty(getUserByEmail.EmailVerificationToken))
            {
                return new ApiResponse<bool>(false, message: ResourceString.Error, apiName: "ResendVerificationLink");
            }

            await _notificationService.EmailVerification(tokenRequest.Email, getUserByEmail.DisplayName ?? "", tokenRequest.Token, ResourceString.ResendVerificationLinkSubject, 0);
            return new ApiResponse<bool>(true, message: ResourceString.ResendVerificationLink, apiName: "ResendVerificationLink");
        }

        public async Task<ApiResponse<ForgotPasswordDto>> ResetPasswordTokenAsync(long userId)
        {
            ApiResponse<ForgotPasswordDto> response = new();
            var userDetail = await _accountRepository.ResetPasswordTokenAsync(userId, Guid.NewGuid().ToString());

            if (userDetail is null)
            {
                response.Message = ResourceString.UserNotExist;
                return response;
            }

            switch ((ForgotPasswordResponseTypes)userDetail.IsValid)
            {
                case ForgotPasswordResponseTypes.TokenUpdatedSuccess:
                    response.Message = ResourceString.ForgetPassword;
                    response.Data = userDetail;
                    await _notificationService.SendResetPasswordEmailToWebUser("Forgot Password", userDetail.ForgotPasswordToken, userDetail.Email, userDetail.UserName);
                    break;

                case ForgotPasswordResponseTypes.ForgotEmailAlreadySent:
                    response.Message = ResourceString.ForgotEmailAlreadySent;
                    break;

                default:
                    if (userDetail.IsDeleted)
                    {
                        response.Message = ResourceString.UserAccountDeleted;
                    }
                    else if (!userDetail.IsActive)
                    {
                        response.Message = ResourceString.UserIsNotActive;
                    }
                    else
                    {
                        response.Message = ResourceString.EmailNotVerified;
                    }
                    break;
            }

            return response;
        }

        public async Task<bool> CheckResetPasswordTokenExist(string token) => await _accountRepository.CheckResetPasswordTokenExist(token);

        public async Task<ResponseTypes> ResetPassword(ResetPasswordModel model)
        {
            var getUserDetail = await _accountRepository.GetUserDetailByToken(model.Token);

            if (getUserDetail is null)
            {
                return ResponseTypes.Error;
            }

            if (Encryption.VerifyHash(model.Password, getUserDetail.PasswordHash))
            {
                return ResponseTypes.OldNewPasswordMatched;
            }

            var resetPasswordObj = await _accountRepository.ResetPassword(model);
            return resetPasswordObj > 0 ? ResponseTypes.Success : ResponseTypes.Error;
        }



        public async Task<ApiResponse<bool>> SaveContactUsDetails(ContactUsRequestModel requestModel)
        {
            // save details
            await _accountRepository.AddSupportTicket(requestModel);
            await _notificationService.SendContactUsMailToAdmin(ResourceString.ContactUsSubject, $"{requestModel.FirstName} {requestModel.LastName}", requestModel.Email, requestModel.Query, $"{requestModel.CountryCode}-{requestModel.PhoneNumber}");
            return new ApiResponse<bool>(true, message: ResourceString.ContactUsMailSuccess, apiName: "ContactUs");
        }

        public async Task<ApiResponse<bool>> UpdateEmailVerificationToken(int userId, string email, string name)
        {
            var emailVerificationToken = Guid.NewGuid().ToString();
            var verifyEmail = await _accountRepository.AddUpdateAsync(new UserDetail()
            {
                Id = userId,
                EmailVerifiedToken = emailVerificationToken,
            });
            if (verifyEmail <= 0)
            {
                return new ApiResponse<bool>(false, message: ResourceString.VerificationLinkSentFailed, apiName: "");
            }

            await _notificationService.EmailVerification(email, name, emailVerificationToken, ResourceString.VerifyEmailSubject, 0);
            return new ApiResponse<bool>(true, message: ResourceString.VerificationLinkSent, apiName: "");
        }


        public CheckUserAccessTokenDto CheckUserAccessToken(string accessToken) => _accountRepository.CheckUserAccessToken(accessToken);

        public bool CheckAppVersion(string appVersion, short deviceTypeId) => _accountRepository.CheckAppVersion(appVersion, deviceTypeId);

        public async Task<UserDetail> GetByIdAsync(int userId)
        {
            return await _accountRepository.GetByIdAsync(userId);
        }

        public async Task<UserDetail> GetAdminDetailById(int userId, string accessToken)
        {
            return await _accountRepository.GetAdminDetailById(userId, accessToken);
        }

        public async Task<List<EscortSearchDto>> GetSearchedEscorts()
        {
            var apiResult = await _accountRepository.GetSearchedEscorts();
            apiResult.ForEach(escort =>
            {
                escort.HeightInFeet = CommonFunctions.CentimetresToFeetInchesString(escort.Height);
            });

            return apiResult;
        }

        public async Task<List<EscortSearchDto>> GetFeaturedEscorts(EscortSearchRequest searchRequest)
        {
            var apiResult = await _accountRepository.GetFeaturedEscorts(searchRequest);
            apiResult.ForEach(escort =>
            {
                escort.HeightInFeet = CommonFunctions.CentimetresToFeetInchesString(escort.Height);
            });

            return apiResult;
        }

        public async Task<List<EscortSearchDto>> GetVipEscorts(EscortSearchRequest searchRequest)
        {
            var apiResult = await _accountRepository.GetVipEscorts(searchRequest);
            apiResult.ForEach(escort =>
            {
                escort.HeightInFeet = CommonFunctions.CentimetresToFeetInchesString(escort.Height);
            });

            return apiResult;
        }

        public async Task<List<EscortSearchDto>> GetPopularEscorts(PopularEscortRequest searchRequest)
        {
            var apiResult = await _accountRepository.GetPopularEscorts(searchRequest);
            apiResult.ForEach(escort =>
            {
                escort.HeightInFeet = CommonFunctions.CentimetresToFeetInchesString(escort.Height);
            });

            return apiResult;
        }

        public async Task<List<EscortSearchDto>> GetFavoriteEscorts(EscortSearchRequest searchRequest)
        {
            var apiResult = await _accountRepository.GetFavoriteEscorts(searchRequest);
            apiResult.ForEach(escort =>
            {
                escort.HeightInFeet = CommonFunctions.CentimetresToFeetInchesString(escort.Height);
            });
            return apiResult;
        }

        public async Task<bool> CheckEmailVerificationTokenExist(string token) => await _accountRepository.CheckEmailVerificationTokenExist(token);

        public async Task<int> GetEstablishmentEscortsCount(int establishmentId) => await _accountRepository.GetEstablishmentEscortsCount(establishmentId);



        public async Task<ApiResponse<bool>> LogoutUser(int id, string accessToken)
        {
            var res = await _accountRepository.LogoutUser(id, accessToken);
            if (res > 0)
            {
                return new ApiResponse<bool>(true, message: ResourceString.LogoutSuccess, apiName: "LogoutUser");
            }
            else
            {
                return new ApiResponse<bool>(false, message: ResourceString.SomethingWrong, apiName: "LogoutUser");
            }
        }


        public async Task<ApiResponse<bool>> LogoutAllUser(string accessToken)
        {
            var res = await _accountRepository.LogoutAllUser(accessToken);
            if (res > 0)
            {
                return new ApiResponse<bool>(true, message: ResourceString.LogoutSuccess, apiName: "LogoutUser");
            }
            else
            {
                return new ApiResponse<bool>(false, message: ResourceString.SomethingWrong, apiName: "LogoutUser");
            }
        }

        public async Task<ApiResponse<bool>> PauseEscort(int userId, bool isPause)
        {
            var res = await _accountRepository.PauseEscort(userId, isPause);
            if (res > 0)
            {
                if (isPause)
                {
                    return new ApiResponse<bool>(true, message: ResourceString.PauseProfile, apiName: "PauseEscort");
                }
                else
                {
                    return new ApiResponse<bool>(true, message: ResourceString.PublishProfile, apiName: "PauseEscort");
                }
            }
            else
            {
                return new ApiResponse<bool>(false, message: ResourceString.SomethingWrong, apiName: "PauseEscort");
            }
        }

        public async Task<ApiResponse<bool>> ApproveEscort(int userId, bool isApprove, int loginUserId)
        {
            var res = await _accountRepository.ApproveEscort(userId, isApprove, loginUserId);
            if (res > 0)
            {
                await SaveActivityLog(userId, isApprove ? ActivityType.ApproveProfile : ActivityType.DeniedProfile);

                if (isApprove)
                {
                    return new ApiResponse<bool>(true, message: ResourceString.ApproveProfile, apiName: "ApproveEscort");
                }
                else
                {
                    return new ApiResponse<bool>(true, message: ResourceString.DeniedProfile, apiName: "ApproveEscort");
                }
            }
            else
            {
                return new ApiResponse<bool>(false, message: ResourceString.SomethingWrong, apiName: "ApproveEscort");
            }
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
